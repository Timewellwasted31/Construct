using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
[ExecuteInEditMode]
public class ScalingSphere : MonoBehaviour {

    static float phi = (1f + Mathf.Sqrt(5)) / 2f;
    static float PhiDistanceConstant = new Vector3(0, 1, phi).magnitude;

    MeshRenderer mr;
    MeshFilter mf;

    [SerializeField] float SphereRadius = 1;
    [SerializeField] int SphereDetail = 0;
    [SerializeField] Vector2 UVTileDimensions = 5 * Vector2.one;
    [SerializeField] bool Activate = false;

	// Use this for initialization
	void Start () {
    }

    void Update()
    {
        if (Activate)
        {
            StartCoroutine(ThreadedAssemble());
            Activate = false;
        }
    }
	
    Vector3 PairedPoint(Vector3 origin, bool ReturnLeft)
    {
        Vector3 retvec = Vector3.zero;
        int comp = (ReturnLeft) ? (-1) : (1);
        // high to low, low to zero, zero to high [upper fork]
        retvec.x = Mathf.Sign(origin.x) * ((Mathf.Abs(origin.x) + 1 > 2) ? (0) : (Mathf.Abs(origin.x) + 1));
        if (Mathf.Abs(retvec.x) == 1) retvec.x = comp;
        retvec.y = Mathf.Sign(origin.y) * ((Mathf.Abs(origin.y) + 1 > 2) ? (0) : (Mathf.Abs(origin.y) + 1));
        if (Mathf.Abs(retvec.y) == 1) retvec.y = comp;
        retvec.z = Mathf.Sign(origin.z) * ((Mathf.Abs(origin.z) + 1 > 2) ? (0) : (Mathf.Abs(origin.z) + 1));
        if (Mathf.Abs(retvec.z) == 1) retvec.z = comp;

        return retvec;
    }


    void Convert2ToPhi(ref Vector3 source)
    {
        if (Mathf.Abs(source.x) > 1.1) source.x = Mathf.Sign(source.x) * phi;
        if (Mathf.Abs(source.y) > 1.1) source.y = Mathf.Sign(source.y) * phi;
        if (Mathf.Abs(source.z) > 1.1) source.z = Mathf.Sign(source.z) * phi;
    }

    IEnumerator ThreadedAssemble()
    {
        if (mr == null) mr = GetComponent<MeshRenderer>();
        if (mr == null) { Debug.Log("BadScalingSphere: no MR"); yield break; }
        if (mf == null) mf = GetComponent<MeshFilter>();
        if (mf == null) { Debug.Log("BadScalingSphere: no MF"); yield break; }


        int task = ThreadRunner.CreateThread(new System.Threading.ParameterizedThreadStart(ThreadedSphere), (object)SphereDetail);
        ThreadRunner.StartThread(task);

        while (!ThreadRunner.isComplete(task))
        {
            yield return null;
        }

        ThreadedSimpleMesh myNewMesh = (ThreadedSimpleMesh)ThreadRunner.FetchData(task);

        Mesh newMesh = myNewMesh.GenerateMesh();

        int poly = newMesh.triangles.GetLength(0) / 3;

        AssetDatabase.CreateAsset(newMesh, "Assets/Exported/ScalingSphere-"+SphereRadius+"m-"+ poly +"v.asset");
        Debug.Log("[GENERATEPYRAMID] Exported mesh: /Assets/Exported/ScalingSphere-" + SphereRadius + "m-" + poly + "v.asset");

        mf.sharedMesh = newMesh;

    }

    void ThreadedSphere(object d)
    {
        int DetailLevel = SphereDetail;

        if (DetailLevel < 1) DetailLevel = 1;

        List<Vector3> verts = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<int> tris = new List<int>();
        int index = 0;

        Vector2 uvstep = new Vector2(1.0f / UVTileDimensions.x, 1.0f / UVTileDimensions.y);


        List<Vector3> PointTriplets = new List<Vector3>();

        // phi = (1 + root(5)) / 2 = 1.6180339887498948482045868343656
        float high = (1f + Mathf.Sqrt(5)) / 2f;
        float low = 1.0f;
        float zero = 0.0f;
        float[] settings = new float[3] { zero, low, high };
        // generate the main points
        // z is forward. So...I don't know.

        // 0, +-1, +- phi 



        // using detail level, produce loops for faces.

        /*
      POINTS ON UNIQUE RECTANGLES
      We take any point.
      eg. P1(h,z,l)
      We fork, raising z to high in both positive and negative.
      Low axis goes to zero.
      High axis goes to low.
      eg. P2a(l,+h,z)  P2b(1,-h, z) 
      Low goes to zero.
      eg. P3a(z,+l,h)   P3b(z,-l,h)
        */
        // we should take the y = 0 rectangle, and generate the 8 faces from it's 4 points
        Vector3[] set = new Vector3[4] { new Vector3(1 , 0, 2),
                                         new Vector3(1 , 0, -2),
                                         new Vector3(-1 , 0, -2),
                                         new Vector3(-1 , 0, 2) };


        Vector3 upperorigin = Vector3.zero, upperleft = Vector3.zero, upperright = Vector3.zero, lowerorigin = Vector3.zero, lowerleft = Vector3.zero, lowerright = Vector3.zero;


        int tileIndex = 0;
        Vector2 uv_origin = Vector2.zero;

        for (int i = 0; i < 4; i++)
        {


            // do lower y triangle first.
            // this triangle points up.
            lowerorigin = new Vector3(set[i].x, set[i].y, set[i].z);
            // high to low, low to zero, zero to high [lower fork]
            lowerleft.x = Mathf.Sign(lowerorigin.x) * ((Mathf.Abs(lowerorigin.x) - 1 < 0) ? (Mathf.Sign(lowerorigin.x) * 2) : (Mathf.Abs(lowerorigin.x) - 1));
            lowerleft.y = Mathf.Sign(lowerorigin.y) * ((Mathf.Abs(lowerorigin.y) - 1 < 0) ? (-2) : (Mathf.Abs(lowerorigin.y) - 1));
            lowerleft.z = Mathf.Sign(lowerorigin.z) * ((Mathf.Abs(lowerorigin.z) - 1 < 0) ? (Mathf.Sign(lowerorigin.z) * 2) : (Mathf.Abs(lowerorigin.z) - 1));
            // eg. P1(h,z,l)

            lowerright.x = Mathf.Sign(lowerorigin.x) * ((Mathf.Abs(lowerleft.x) - 1 < 0) ? (2) : (Mathf.Abs(lowerleft.x) - 1));
            lowerright.y = Mathf.Sign(lowerleft.y) * ((Mathf.Abs(lowerleft.y) - 1 < 0) ? (-2) : (Mathf.Abs(lowerleft.y) - 1));
            lowerright.z = Mathf.Sign(lowerorigin.z) * ((Mathf.Abs(lowerleft.z) - 1 < 0) ? (Mathf.Sign(lowerorigin.z) * 2) : (Mathf.Abs(lowerleft.z) - 1));

            Convert2ToPhi(ref lowerorigin); Convert2ToPhi(ref lowerleft); Convert2ToPhi(ref lowerright);

            PointTriplets.Add(lowerorigin);
            PointTriplets.Add(lowerleft);
            PointTriplets.Add(lowerright);

            // begin upper triangles
            // this triangle points up.
            upperorigin = new Vector3(set[i].x, set[i].y, set[i].z);

            // these calculations are chained, and thus don't do well in their own functions.
            // high to low, low to zero, zero to high [upper fork]
            upperleft.x = Mathf.Sign(upperorigin.x) * ((Mathf.Abs(upperorigin.x) - 1 < 0) ? (Mathf.Sign(upperorigin.x) * 2) : (Mathf.Abs(upperorigin.x) - 1));
            upperleft.y = Mathf.Sign(upperorigin.y) * ((Mathf.Abs(upperorigin.y) - 1 < 0) ? (2) : (Mathf.Abs(upperorigin.y) - 1));
            upperleft.z = Mathf.Sign(upperorigin.z) * ((Mathf.Abs(upperorigin.z) - 1 < 0) ? (Mathf.Sign(upperorigin.z) * 2) : (Mathf.Abs(upperorigin.z) - 1));
            // eg. P1(h,z,l)

            upperright.x = Mathf.Sign(upperorigin.x) * ((Mathf.Abs(upperleft.x) - 1 < 0) ? (2) : (Mathf.Abs(upperleft.x) - 1));
            upperright.y = Mathf.Sign(upperleft.y) * ((Mathf.Abs(upperleft.y) - 1 < 0) ? (2) : (Mathf.Abs(upperleft.y) - 1));
            upperright.z = Mathf.Sign(upperorigin.z) * ((Mathf.Abs(upperleft.z) - 1 < 0) ? (Mathf.Sign(upperorigin.z) * 2) : (Mathf.Abs(upperleft.z) - 1));

            Convert2ToPhi(ref upperorigin); Convert2ToPhi(ref upperleft); Convert2ToPhi(ref upperright);

            PointTriplets.Add(upperorigin);
            PointTriplets.Add(upperright);
            PointTriplets.Add(upperleft);

        }

        /*
           END OF POINTS ON UNIQUE RECTANGLES ALGORITHM
        */

        /*
           BEGIN PAIRED POINTS ALGORITHM
        */
        set = new Vector3[12] {
                    new Vector3(1 , 0, 2), new Vector3(1 , 0, -2), new Vector3(-1 , 0, -2), new Vector3(-1 , 0, 2), // this is right
                    new Vector3(2, 1, 0), new Vector3(-2, 1, 0), new Vector3(-2, -1, 0), new Vector3(2, -1, 0),
                    new Vector3(0, 2, 1), new Vector3(0, -2, 1), new Vector3(0, -2, -1), new Vector3(0, 2, -1)
        };

        Vector3 origin = Vector3.zero, leftpoint = Vector3.zero, rightpoint = Vector3.zero;
        for (int i = 0; i < set.Length; i++)
        {
            origin = new Vector3(set[i].x, set[i].y, set[i].z);
            leftpoint = PairedPoint(origin, true);
            rightpoint = PairedPoint(origin, false);

            Convert2ToPhi(ref origin); Convert2ToPhi(ref leftpoint); Convert2ToPhi(ref rightpoint);

            PointTriplets.Add(origin);
            PointTriplets.Add(leftpoint);
            PointTriplets.Add(rightpoint);

        }

        for (int i = 0; i < PointTriplets.Count / 3; i++)
        {
            //  TEXTURE CONTROL 
            tileIndex = i;
            uv_origin = new Vector2(uvstep.x * (tileIndex % UVTileDimensions.x), 1.0f - uvstep.y * (1 + Mathf.Floor(tileIndex / UVTileDimensions.x)));

            origin = PointTriplets[i * 3];
            leftpoint = PointTriplets[i * 3 + 1];
            rightpoint = PointTriplets[i * 3 + 2];

            Vector3 leftstep = (leftpoint - origin) / DetailLevel;
            Vector3 rightstep = (rightpoint - origin) / DetailLevel;

            //  END TEXTURE CONTROL 

            bool oddTriangle = false; // odd triangles are upside down.
            Vector3 myOrigin, leftfoot, rightfoot, temp;
            bool faceValue;
            for (int row = 0; row < DetailLevel; row++)
            {

                myOrigin = origin + row * leftstep;
                leftfoot = myOrigin + leftstep;
                rightfoot = myOrigin + rightstep;

                // wizard shit. No idea.
                faceValue = (Mathf.Sign(origin.x) * Mathf.Sign(origin.z) == ((origin.y != 0) ? (Mathf.Sign(origin.y)) : (1)));

                for (int n = 0; n < 1 + 2 * row; n++)
                {
                    // Debug.Log("triangle #" + (n + 1) + " m: " + myOrigin + " l: " + leftfoot + " r:" + rightfoot);
                    index = verts.Count;
                    verts.Add(myOrigin);
                    verts.Add(leftfoot);
                    verts.Add(rightfoot);

                    // Wizard shit. Seriously. No idea. It just works.
                    if (!faceValue)
                    {
                        tris.Add(index); tris.Add(index + 2); tris.Add(index + 1);
                    }
                    else {
                        tris.Add(index); tris.Add(index + 1); tris.Add(index + 2);
                    }

                    if (oddTriangle)
                    {
                        temp = myOrigin;
                        myOrigin = leftfoot;
                        leftfoot = temp;
                        rightfoot = myOrigin + rightstep;
                    }
                    else {
                        temp = myOrigin;
                        myOrigin = rightfoot;
                        leftfoot = myOrigin - leftstep;
                        rightfoot = temp;
                    }

                    uvs.Add(new Vector2(uv_origin.x + 0.5f * uvstep.x, uv_origin.y + uvstep.y));
                    if (tileIndex % 2 != 0)
                    {
                        uvs.Add(new Vector2(uv_origin.x + 0f, uv_origin.y + 0f));
                        uvs.Add(new Vector2(uv_origin.x + uvstep.x, uv_origin.y + 0f));
                    }
                    else {
                        uvs.Add(new Vector2(uv_origin.x + uvstep.x, uv_origin.y + 0f));
                        uvs.Add(new Vector2(uv_origin.x + 0f, uv_origin.y + 0f));
                    }

                    oddTriangle = !oddTriangle;
                }

                oddTriangle = false;
            }
        }

     

        // apply proper scaling.
        for (int i = 0; i < verts.Count; i++)
        {
            Vector3 temp = verts[i];
            temp = (PhiDistanceConstant / temp.magnitude) * temp; // sets all points to the phyDistanceConstant.
            temp = (SphereRadius / PhiDistanceConstant) * temp; // sets all points to sphere radius.
            verts[i] = temp;

        }

        // export final verts, tris and uvs to mesh and wrap it up.

        ThreadedSimpleMesh newMesh = new ThreadedSimpleMesh();

        newMesh.SetVertices(verts);
        newMesh.SetTriangles(tris, 0);
        newMesh.SetUVs(0, uvs);

        // newMesh.RecalculateBounds();
        // newMesh.RecalculateNormals();

        ThreadRunner.ExportData(newMesh);
        ThreadRunner.MarkComplete();
        
    }

}
