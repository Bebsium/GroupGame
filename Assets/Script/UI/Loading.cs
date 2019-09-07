using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Loading : MonoBehaviour
{
    public bool run = true;

    [SerializeField]
    Sprite[] texture;
    [SerializeField]
    Image image;
    
    void Start()
    {
        StartCoroutine(Run());
    }

    IEnumerator Run()
    {
        Color a = new Color(1, 1, 1, 0);
        while(image.color.a < 1 && run)
        {
            yield return new WaitForSeconds(Time.deltaTime);
            image.color = a;
            a.a += Time.deltaTime * 0.8f;
            //image.color = Color.Lerp(image.color, Color.white, Time.deltaTime);
        }
        image.color = Color.white;
        float n = 1;
        while (run)
        {
            yield return new WaitForSeconds(Time.deltaTime);
            image.sprite = texture[(int)n];
            n += Time.deltaTime * 2f;
            if (n > 5) n = 0;
        }
        image.sprite = texture[4];
        yield return new WaitForSeconds(1);
        image.sprite = texture[5];
        yield return new WaitForSeconds(2);
        Destroy(gameObject);
    }
}
