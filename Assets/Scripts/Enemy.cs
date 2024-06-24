using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MovingObject
{
    /* public 변수 */
    public int playerDamage;    // Player 공격시 감소시킬 배고픔 수치

    // Enemy가 사용할 오디오 클립들
    public AudioClip enemyAttack1;
    public AudioClip enemyAttack2;

    /* private 변수 */
    private Animator animator;  // 오브젝트의 애니메이터 레퍼런스
    private Transform target;   // Player의 위치
    private bool skipMove;      // Enemy가 턴마다 움직이게 하는 데 쓰이는 변수

    protected override void Start()
    {
        GameManager.instance.AddEnemyToList(this);
        animator = GetComponent<Animator>();
        target = GameObject.FindGameObjectWithTag("Player").transform;

        base.Start();
    }

    /* 오버라이딩한 함수들 */
    // 이동할 수 있으면 이동하고, 이동할 수 없으면 OnCantMove를 실행하는 함수
    protected override void AttempMove<T>(int xDir, int yDir)
    {
        if (skipMove)
        {
            skipMove = false;
            return;
        }
        base.AttempMove<T>(xDir, yDir);

        skipMove = true;
    }

    // 이동하려는 위치에 상호작용할 수 있는 오브젝트(Player)가 있을 때 실행되는 함수
    protected override void OnCantMove<T>(T component)
    {
        Player hitPlayer = component as Player;                         // T 타입의 component를 Player로 타입 캐스팅하기

        hitPlayer.LoseFood(playerDamage);

        animator.SetTrigger("enemyAttack");

        SoundManager.instance.RandomizeSfx(enemyAttack1, enemyAttack2); // 공격 소리 재생하기
    }

    /* public 함수들 */
    // GameManager에서 각 Enemy오브젝트마다 실행시키는 함수
    public void MoveEnemy()
    {
        int xDir = 0;                                                               // x축 이동
        int yDir = 0;                                                               // y축 이동

        if (Mathf.Abs(target.position.x - transform.position.x) < float.Epsilon)    // 만약 target과 나의 x좌표가 같다면
            yDir = target.position.y > transform.position.y ? 1 : -1;                   // target의 y좌표가 나의 y좌표보다 크면 y축 이동을 1, 아니면 -1로 정하기
        else                                                                        // 만약 x좌표가 다르다면
            xDir = target.position.x > transform.position.x ? 1 : -1;                   // target의 x좌표가 나의 x좌표보다 크면 x축 이동을 1, 아니면 -1로 정하기

        AttempMove<Player>(xDir, yDir);
    }
}
