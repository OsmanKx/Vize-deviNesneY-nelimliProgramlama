using System;
using UnityEngine;

public class FoodObject : CellObject
{
    public int AmountGranted = 10;
    public override void PlayerEntered()
    {
        Debug.Log("Player entered food");
        Destroy(gameObject);
        Debug.Log("Food increased");



        GameManager.Instance.ChangeFood(AmountGranted);
    }

    
}
