using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    public Sprite dmgSprite;
    public int hp = 3;
    public int recoverAmount = 5;  // ȸ����

    private SpriteRenderer spriteRenderer;
    private Player player;  // �÷��̾� ����

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();  // �÷��̾� ã��
    }

    public void DamageWall(int loss)
    {
        spriteRenderer.sprite = dmgSprite;
        hp -= loss;
        if (hp <= 0)
        {
            gameObject.SetActive(false);

            // 50% Ȯ���� ü�� ȸ��
            if (Random.value < 0.5f)
            {
                player.RecoverHealth(recoverAmount);
            }
        }
    }
}