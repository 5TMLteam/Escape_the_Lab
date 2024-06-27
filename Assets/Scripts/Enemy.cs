using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Enemy : MovingObject
{
    /* public ���� */
    public int playerDamage;    // Player ���ݽ� ���ҽ�ų ����� ��ġ

    // Enemy�� ����� ����� Ŭ����
    public AudioClip enemyAttack1;
    public AudioClip enemyAttack2;
    public Text skipMoveText;

    /* private ���� */
    private Animator animator;  // ������Ʈ�� �ִϸ����� ���۷���
    private Transform target;   // Player�� ��ġ
    private bool skipMove;      // Enemy�� �ϸ��� �����̰� �ϴ� �� ���̴� ����
    private Transform canvas;
    //private Text skipMoveText;

    protected override void Start()
    {
        GameManager.instance.AddEnemyToList(this);
        animator = GetComponent<Animator>();
        target = GameObject.FindGameObjectWithTag("Player").transform;

        skipMoveText = Instantiate(skipMoveText, target, transform);                    // skipMove���� ǥ���ϴ� �ؽ�Ʈ �����ϱ�
        canvas = GameObject.Find("Canvas").GetComponent<Canvas>().transform;
        skipMoveText.rectTransform.SetParent(canvas);

        base.Start();
    }

    /* �������̵��� �Լ��� */
    // �̵��� �� ������ �̵��ϰ�, �̵��� �� ������ OnCantMove�� �����ϴ� �Լ�
    protected override void AttempMove<T>(int xDir, int yDir)
    {
        if (skipMove)
        {
            skipMoveText.text = "";
            skipMove = false;
            return;
        }

        base.AttempMove<T>(xDir, yDir);

        // ������ �̵� ���Ѵٴ� ǥ���ϱ�
        RectTransform text = skipMoveText.rectTransform;
        Vector2 unit = canvas.position * 2 / 10;
        text.position = new Vector2((transform.position.x + 1.8f) * unit.x, (transform.position.y + 1.9f) * unit.y);
        if (base.canMove) // SmoothMove�� �̵��� �ݿ��� �ȵǾ �߰�
            text.position = text.position + new Vector3(xDir * unit.x, yDir * unit.y);
        skipMoveText.text = "...";

        skipMove = true;
    }

    // �̵��Ϸ��� ��ġ�� ��ȣ�ۿ��� �� �ִ� ������Ʈ(Player)�� ���� �� ����Ǵ� �Լ�
    protected override void OnCantMove<T>(T component)
    {
        Player hitPlayer = component as Player;                         // T Ÿ���� component�� Player�� Ÿ�� ĳ�����ϱ�

        hitPlayer.LoseFood(playerDamage);

        animator.SetTrigger("enemyAttack");

        SoundManager.instance.RandomizeSfx(enemyAttack1, enemyAttack2); // ���� �Ҹ� ����ϱ�
    }

        /* public �Լ��� */
        // GameManager���� �� Enemy������Ʈ���� �����Ű�� �Լ�
        public void MoveEnemy()
    {
        int xDir = 0;                                                               // x�� �̵�
        int yDir = 0;                                                               // y�� �̵�

        if (Mathf.Abs(target.position.x - transform.position.x) < float.Epsilon)    // ���� target�� ���� x��ǥ�� ���ٸ�
            yDir = target.position.y > transform.position.y ? 1 : -1;                   // target�� y��ǥ�� ���� y��ǥ���� ũ�� y�� �̵��� 1, �ƴϸ� -1�� ���ϱ�
        else                                                                        // ���� x��ǥ�� �ٸ��ٸ�
            xDir = target.position.x > transform.position.x ? 1 : -1;                   // target�� x��ǥ�� ���� x��ǥ���� ũ�� x�� �̵��� 1, �ƴϸ� -1�� ���ϱ�

        AttempMove<Player>(xDir, yDir);
    }
}
