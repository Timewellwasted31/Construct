using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * The basic worm class.
 * TODO: Advanced worm class: 
 *  SPITTER: Include projectile classes. 
 *  BURROWER: Normal movement behaviour places the enemy underground.
 *  BOUNCER: When hit, segments fall off and bounce around menacingly.
 */
public class WormEnemy : ConstructEnemy
{
    static List<WormEnemy> EnemyWormList = new List<WormEnemy>();

    [SerializeField] List<Transform> HeadToTail;
    [SerializeField] float SegmentSpacing = 1.5f;

    // movement variables
    [Header("Movement")]
    [SerializeField] float SinOctave; // sets the wavelength
    [SerializeField] float WaveAmplitude; // sets the amplitude
    [SerializeField] float WaveDelay; // sets the offset between segments.
    [SerializeField] float TransformDampen = 0.95f; // lerp for segment position
    [SerializeField] float OctaveDampen = 1f; // lerp for segment position
    [SerializeField] float AmpDampen = 1f; // lerp for segment position
    [SerializeField] float delta_amp = 2f;

    // leap attack
    [Header("Leap")]
    [SerializeField] float LeapSpeed = 1.0f; // sets the speed during leap movement.
    Vector3 LeapTarget; // holds terminal point for the leap attack
    float LeapHeight; // holds the highest height attainable
    float LeapLerp; // our progression through the leap cycle.
    float LeapDistance; // holds the distance to the target
    float LeapApex; // 0 is the middle. Negative values are jumping up, and reach apex later. Postive values, earlier.
    float LeapTime;

    float CurAmplitude, CurOctave;
    float delta;


    // gives this monster a leap attack target, and sets the leap height.
    // leap height is at the midpoint of a leap.
    protected override void LeapAttack(Vector3 WorldTarget, float Height) {
        AIAttackStatus = EnemyAttackStatus.Leap;
        LeapTarget = WorldTarget;
        LeapHeight = Height;
        LeapDistance = (WorldTarget - transform.position).magnitude;
        LeapApex = (transform.position.y - LeapTarget.y)/LeapHeight;

        LeapLerp = 0;
        LeapTime = 0;
    }



    // Use this for initialization
    void Start () {
        base.Start();

        EnemyWormList.Add(this);

        CurAmplitude = 0; CurOctave = 0; delta = Random.value * 30f;

    }

    int counter = 1;

    // Update is called once per frame
    new void Update () {
        base.Update();

        if (EnemyInBin) return;

        delta += delta_amp * Time.deltaTime;

        if (delta > 5 * counter && false)
        {
            counter++;
            if (AIAttackStatus != EnemyAttackStatus.Leap)
            {
                Vector3 worldTarget = transform.position + 5f * transform.forward;
                LeapAttack(worldTarget, 5f);
            }
        }

        if (AIAttackStatus == EnemyAttackStatus.None)
        {
            MovementAnimation(); // default movement behaviour.
        }else if (AIAttackStatus == EnemyAttackStatus.Leap)
        {
            LeapAnimation();
        }

	}

    void LeapAnimation()
    {
        LeapTime += Time.deltaTime; // advance lerp function by one frame.

        // while leaping, the worm's segment all stay in line on the x axis.
        // we design a parabolic formula describing the jump
        // we then place segments along this path, shifting based on the segment coherence value.

        // how do we determine the parabola?

        // if the origin is higher than the destination, max height is attained earlier.
        // ratio, difference in height over height of jump?
        // if we jump 10 m down, 10m away with a 1m height, then we should reach maximum height at -10 / 1 towa
        // we'll experiment.

        // we need to know how long the leap should last. And therefore, we need to know how far the leap is and the speed at which we leap.
        float t = LeapDistance / (LeapSpeed); // total time to leap, rough.

        LeapLerp = LeapTime / t;
        float myLeapLerp = 2f;

        for (int i = 0; i < HeadToTail.Count; i++)
        {

            myLeapLerp = LeapLerp - (i*(SegmentSpacing/LeapDistance)); // let's try this
            Vector3 cur = HeadToTail[i].transform.localPosition;

            // okay, fuck it. we have myLeapLerp, let's just do this.
            if (myLeapLerp < 0 || myLeapLerp > 1)
            {
                // if the calculated LeapLerp value for this node is negative or greater than 1 (completed), then we eliminate the vertical offset
                // and move the node toward the centerline.
                HeadToTail[i].transform.localPosition = Vector3.Lerp(cur, new Vector3(0, 0, cur.z), 2 * Time.deltaTime * TransformDampen);
                
                continue;
            }

            // otherwise, we set the vertical position within the group
            //
            HeadToTail[i].transform.localPosition = Vector3.Lerp(cur, new Vector3(0, (1-Mathf.Abs(1 - 2* myLeapLerp))*LeapHeight, cur.z), 2*Time.deltaTime * TransformDampen);
            

        }

        if (myLeapLerp > 1)
        {
            Debug.Log("Ending jump cycle.");
            AIAttackStatus = EnemyAttackStatus.None;
        }

    }

    void MovementAnimation()
    {

        if (SinOctave <= 0) SinOctave = 1;
        if (CurOctave <= 0) CurOctave = 1;


        CurOctave = Mathf.Lerp(CurOctave, SinOctave, OctaveDampen * Time.deltaTime);
        CurAmplitude = Mathf.Lerp(CurAmplitude, WaveAmplitude, AmpDampen * Time.deltaTime);


        for (int i = 0; i < HeadToTail.Count; i++)
        {

            Vector3 n = CurAmplitude * Mathf.Sin((Movement.z * delta + WaveDelay * i) / CurOctave) * Vector3.right + SegmentSpacing * Vector3.back * i;
            Vector3 CurPosition = HeadToTail[i].transform.localPosition;
            HeadToTail[i].transform.localPosition = Vector3.Lerp(CurPosition, n, TransformDampen * Time.deltaTime);
        }
    }
}
