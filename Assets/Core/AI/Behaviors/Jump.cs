using Core.AI;
using Core.Character;
using DG.Tweening;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;
using BehaviorDesigner.Runtime;

[TaskCategory("Move/Jump")]
public class Jump : EnemyAction
{
    [LinkedTask]
    public SharedInt lianjie;
    public Jump[] linkedjump = null;
    public float horizontalForce = 5.0f;
    public float jumpForce = 10.0f;

    public float buildupTime;
    public float jumpTime;

    public string animationTriggerName;
    public bool shakeCameraOnLanding;

    private bool hasLanded;

    private Tween buildupTween;
    private Tween jumpTween; // 类的成员变量

    public SharedInt ad;

    // 攻击配置
    public int jumpAttake =1;
    
    public Vector2 JumpAttakeOffset ;
    public float JumpAttakeRange;
    public LayerMask PlayerLayer;

    public override void OnStart()
    {
        hasLanded = false; // 每次开始重置
        buildupTween = DOVirtual.DelayedCall(buildupTime, StartJump, false);
        animator.SetTrigger(animationTriggerName);
    }

    private void StartJump()
    {
        var direction = player.transform.position.x < transform.position.x ? -1f : 1f;
        body.AddForce(new Vector2(horizontalForce * direction, jumpForce), ForceMode2D.Impulse);

        // ✅ 这里不能写 Tween jumpTween
        // ✅ 直接用类的成员变量
        jumpTween = DOVirtual.DelayedCall(jumpTime, () =>
        {
            hasLanded = true; // ✅ 现在一定会执行

            JumpAttake(); // ✅ 落地伤害

            if (shakeCameraOnLanding)
                CameraController.Instance.ShakeCamera(0.5f);
        }, false);
    }

    private void JumpAttake()
    {
        Vector2 AttakePoint = (Vector2)transform.position + JumpAttakeOffset;
        Collider2D[] hits = Physics2D.OverlapCircleAll(AttakePoint, JumpAttakeRange, PlayerLayer);

        foreach (Collider2D hit in hits)
        {
            Player player = hit.GetComponent<Player>();
            if (player != null)
            {
                player.HitPlayer(jumpAttake);
                Debug.Log("打中玩家！");
                // player.TakeDamage(1);
            }
        }
    }

    public override TaskStatus OnUpdate()
    {
        // 落地 → 返回 Success
        if (hasLanded)
            return TaskStatus.Success;

        return TaskStatus.Running;
    }

    public override void OnEnd()
    {
        buildupTween?.Kill();
        jumpTween?.Kill(); // 现在能正确销毁
        hasLanded = false;
    }

    // 编辑器显示攻击范围（不用调用，Unity自动调用）
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector2 center = (Vector2)transform.position + JumpAttakeOffset;
        Gizmos.DrawWireSphere(center, JumpAttakeRange);
    }
}
