using UnityEngine;

public class PimpkinHead : MonoBehaviour
{

    private bool IsHit;
    private RectTransform PimpkinBody;
    private Vector3 StartingPosition;
    private int Switch = 0;
    private float RandomIntX;
    private float RandomIntY;
    private float Speed = 300f;
    [SerializeField] AudioSource PipmkinDeathSource;

    void Start()
    {
        IsHit = false;
        PimpkinBody = GetComponent<RectTransform>();
        gameObject.SetActive(true);
        RandomIntX = Random.Range(-1, 1) + 0.5f; 
        RandomIntY = Random.Range(0, 1) + 0.5f;
    }

    void Update()
    {
        MovePimpkinHead();
    }

    private void MovePimpkinHead()
    {
        Switch++;

        if (Switch % 120 == 0)
        {
            RandomIntX = Random.Range(-2, 2);
            RandomIntY = Random.Range(0, 2);
        }

        PimpkinBody.transform.position += new Vector3(RandomIntX, RandomIntY, 0) * (Speed * Time.deltaTime);
    }

    public void DestroyPimpkin()
    {
        Debug.Log("Hit pimpkin!!!!!");
        PipmkinDeathSource.Play();
        gameObject.SetActive(false);
        IsHit = true;
    }

    public void ResetPimpkinValues()
    {
        IsHit = false;
        Switch = 0;
    }

    public bool GetHit()
    {
        return IsHit;
    }
}
