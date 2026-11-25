using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using BackEnd;
using LitJson;

public class CouponItem
{
    public string uuid;
    public string chartFileName;
    public int Data_Id;
    public int itemCount;
    public override string ToString()
    {
        return $"itemID : {Data_Id}\n" +
        $"itemCount : {itemCount}\n" +
        $"chartFileName : {chartFileName}\n" +
        $"uuid : {uuid}\n";
    }
}

public class BackendCoupon
{
    public void UseCoupon(string couponCode, Action<BackendReturnObject, List<CouponItem>> complete = null)
    {
        Backend.Coupon.UseCoupon(couponCode, callback =>
        {
            List<CouponItem> couponItemList = null;
            if (callback.IsSuccess())
            {
                couponItemList = new List<CouponItem>();
                JsonData json = callback.GetReturnValuetoJSON();
                JsonData jItemObjects = json["itemObject"];
                for (int i = 0; i < jItemObjects.Count; i++)
                {
                    JsonData jItemObject = jItemObjects[i];

                    CouponItem couponItem = new CouponItem();
                    couponItem.uuid = json["uuid"].ToString();
                    couponItem.itemCount = GameUtils.GameUtils.TryParse<int>("itemCount", jItemObject);

                    JsonData jItem = jItemObject["item"];
                    couponItem.Data_Id = GameUtils.GameUtils.TryParse<int>("Data_Id", jItem);
                    couponItem.chartFileName = jItem["chartFileName"].ToString();

                    couponItemList.Add(couponItem);
                }
            }
            complete?.Invoke(callback, couponItemList);
        });
    }

}
