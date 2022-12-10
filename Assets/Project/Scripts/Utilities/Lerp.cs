using System.Collections;
using UnityEngine;

public class Lerp : MonoBehaviour
{
    [Header("LERP FUNCTIONS")]
    [SerializeField] AnimationCurve colorLerpFunction;
    [SerializeField] AnimationCurve positionLerpFunction;
    [SerializeField] AnimationCurve scaleLerpFunction;

    //Color lerp varialbes
    [HideInInspector] public bool finishedColorLerp;

    private Color32 startingColor;
    private Color32 colorGoalVect;

    private float colorCurrentTime;
    private float colorLerpTime;

    private bool fadeOut;
    private SpriteRenderer sr;

    //Position lerp variables
    [HideInInspector] public bool finishedPositionLerp;
    private bool invertPosFunction;

    private Vector3 startingVect;
    private Vector3 dirPosVect;

    private float posCurrentTime;
    private float posLerpTime;

    private Transform transformTarget;

    //private Rigidbody2D rb;

    //Scale lerp variables
    [HideInInspector] public bool finishedScaleLerp;

    private Vector3 startingScale;
    private Vector3 dirScaleVect;

    private float scaleCurrentTime;
    private float scaleLerpTime;


    void Awake()
    {
        if (GetComponent<Rigidbody2D>() != null)
        {
            //rb = GetComponent<Rigidbody2D>();
        }
        if (GetComponent<SpriteRenderer>() != null)
        {
            sr = GetComponent<SpriteRenderer>();
        }
    }

    //Color lerp functions
    public void LerpColor(Color32 colorGoal, float lTime)
    {
        colorLerpTime = lTime;

        startingColor = sr.color;
        colorGoalVect = colorGoal - sr.color;
        finishedColorLerp = false;

        if (gameObject.activeInHierarchy)
        {
            StartCoroutine(LerpColor());
        }
    }

    public void FadeOut(float lTime)
    {
        fadeOut = true;
        colorLerpTime = lTime;

        startingColor = sr.color;
        finishedColorLerp = false;

        if (gameObject.activeInHierarchy)
        {
            StartCoroutine(FadeAlpha());
        }
    }

    public void FadeIn(float desiredAlpha, float lTime)
    {
        fadeOut = false;
        colorLerpTime = lTime;

        if (sr == null)
        {
            sr = GetComponent<SpriteRenderer>();
        }
        startingColor = sr.color;
        colorGoalVect = new Color(sr.color.r, sr.color.g, sr.color.b, desiredAlpha);
        finishedColorLerp = false;

        if (gameObject.activeInHierarchy)
        {
            StartCoroutine(FadeAlpha());
        }
    }

    IEnumerator FadeAlpha()
    {
        colorCurrentTime = 0.0f;
        float alpha;

        while (colorCurrentTime < colorLerpTime)
        {
            colorCurrentTime += Time.deltaTime;

            float tParam = colorLerpFunction.Evaluate(colorCurrentTime / colorLerpTime);

            if (fadeOut)
            {
                alpha = startingColor.a - startingColor.a * tParam;
            }
            else
            {
                alpha = colorGoalVect.a * tParam;
            }

            sr.color = new Color32(startingColor.r, startingColor.g, startingColor.b, (byte)alpha);

            yield return null;
        }

        finishedColorLerp = true;
    }

    IEnumerator LerpColor()
    {
        colorCurrentTime = 0.0f;

        while (colorCurrentTime < colorLerpTime)
        {
            colorCurrentTime += Time.deltaTime;

            float tParam = colorLerpFunction.Evaluate(colorCurrentTime / colorLerpTime);

            float red = startingColor.r + colorGoalVect.r * tParam;
            float green = startingColor.g + colorGoalVect.g * tParam;
            float blue = startingColor.b + colorGoalVect.b * tParam;
            float alpha = startingColor.a + colorGoalVect.a * tParam;

            sr.color = new Color32((byte)red, (byte)green, (byte)blue, (byte)alpha);

            yield return null;
        }

        finishedColorLerp = true;
    }

    //Position lerp functions
    public void LerpPosition(Vector3 positionToGo, float lTime, bool invert)
    {
        posLerpTime = lTime;
        invertPosFunction = invert;

        TrueLerpPosition(positionToGo);
    }

    public void LerpPosition(Vector3 positionToGo, float lTime)
    {
        posLerpTime = lTime;
        invertPosFunction = false;

        TrueLerpPosition(positionToGo);
    }

    public void LerpPosition(Transform newTransformTarget, float lerpSpeed)
    {
        posLerpTime = ((newTransformTarget.position - transform.position).magnitude / 15.0f) / lerpSpeed;
        Debug.Log(posLerpTime);
        invertPosFunction = false;
        transformTarget = newTransformTarget;

        TrueLerpPosition(transformTarget.position);
    }

    public void SpeedLerpPosition(Vector3 positionToGo, float lerpSpeed)
    {
        //posLerpTime = ((positionToGo - transform.position).magnitude / 15.0f) * lerpSpeed;
        posLerpTime = ((positionToGo - transform.position).magnitude / 15.0f) / lerpSpeed;
        invertPosFunction = false;

        TrueLerpPosition(positionToGo);
    }

    public void LerpPosition(Vector3 positionToGo)
    {
        posLerpTime = (positionToGo - transform.position).magnitude / 15.0f;
        invertPosFunction = false;

        TrueLerpPosition(positionToGo);
    }

    void TrueLerpPosition(Vector3 positionToGo)
    {
        startingVect = transform.position;
        dirPosVect = positionToGo - startingVect;
        finishedPositionLerp = false;

        if (gameObject.activeInHierarchy)
        {
            if (!invertPosFunction)
            {
                StartCoroutine("RearrangeNow");
            }
            else
            {
                StartCoroutine("InvertedRearrangeNow");
            }
        }
    }

    IEnumerator RearrangeNow()
    {
        posCurrentTime = 0.0f;

        while (posCurrentTime < posLerpTime)
        {
            if(transformTarget != null)
            {
                dirPosVect = transformTarget.position - startingVect;
            }

            posCurrentTime += Time.deltaTime;

            float tParam = positionLerpFunction.Evaluate(posCurrentTime / posLerpTime);

            //rb.MovePosition(startingVect + dirPosVect * tParam);
            transform.position = startingVect + (dirPosVect * tParam);

            yield return null;
        }

        finishedPositionLerp = true;
        //rb.velocity = Vector2.zero;
        transformTarget = null;
    }

    IEnumerator InvertedRearrangeNow()
    {
        posCurrentTime = posLerpTime;

        while (posCurrentTime > 0.0f)
        {
            posCurrentTime -= Time.deltaTime;

            float tParam = 1 - positionLerpFunction.Evaluate(posCurrentTime / posLerpTime);

            //rb.MovePosition(startingVect + dirPosVect * tParam);
            transform.position = startingVect + (dirPosVect * tParam);

            yield return null;
        }

        finishedPositionLerp = true;
        //rb.velocity = Vector2.zero;
    }

    //Scale lerp functions
    public void LerpScale(Vector3 scaleToGo, float lTime)
    {
        scaleLerpTime = lTime;

        startingScale = transform.localScale;
        dirScaleVect = scaleToGo - transform.localScale;
        finishedScaleLerp = false;

        if (gameObject.activeInHierarchy)
        {
            StartCoroutine(ScaleNow());
        }
    }
    public void LerpUnscaled(Vector3 scaleToGo, float lTime)
    {
        scaleLerpTime = lTime;

        startingScale = transform.localScale;
        dirScaleVect = scaleToGo - transform.localScale;
        finishedScaleLerp = false;

        if (gameObject.activeInHierarchy)
        {
            StartCoroutine(UnScaledScaledNow());
        }
    }
    IEnumerator ScaleNow()
    {
        scaleCurrentTime = 0.0f;

        while (scaleCurrentTime < scaleLerpTime)
        {
            scaleCurrentTime += Time.deltaTime;

            float tParam = scaleLerpFunction.Evaluate(scaleCurrentTime / scaleLerpTime);

            transform.localScale = startingScale + dirScaleVect * tParam;

            yield return null;
        }

        finishedScaleLerp = true;
    }
    IEnumerator UnScaledScaledNow()
    {
        scaleCurrentTime = 0.0f;

        while (scaleCurrentTime < scaleLerpTime)
        {
            scaleCurrentTime += Time.unscaledDeltaTime;

            float tParam = scaleLerpFunction.Evaluate(scaleCurrentTime / scaleLerpTime);

            transform.localScale = startingScale + dirScaleVect * tParam;

            yield return null;
        }

        finishedScaleLerp = true;
    }
}
