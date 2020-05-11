using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthCollectible : MonoBehaviour
{
    // 不需要初始化及按帧刷新 可以删除Start() Update() 方法
    public AudioClip collectedClip;
    void OnTriggerEnter2D(Collider2D other)
    {
        RubyController controller = other.GetComponent<RubyController>();

        if (controller!=null && controller.Health < controller.maxHealth)
        {
            controller.ChangeHealth(1);
            Destroy(gameObject); // gameObject 代表当前对象
            //This is the GameObject that the script is attached to (the collectible health pack)
            controller.PlaySound(collectedClip);
        }
    }

}
