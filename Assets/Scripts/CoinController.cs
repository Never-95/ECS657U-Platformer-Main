using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinController : MonoBehaviour
{
    public void DeactivateCoin(GameObject coinObject, float effectDuration)
    {
        Debug.Log(coinObject);
        StartCoroutine(Deactivate(coinObject, effectDuration));
    }

    private System.Collections.IEnumerator Deactivate(GameObject coinObject, float effectDuration)
    {
        Debug.Log("deactivated!");
        coinObject.SetActive(false);
        yield return new WaitForSeconds(effectDuration);
        Debug.Log("activated!");
        coinObject.SetActive(true);
    }
}
