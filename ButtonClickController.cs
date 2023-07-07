//简易思考动画的触发和平滑过渡
using UnityEngine;

public class ButtonClickController : MonoBehaviour
{
    public Animator modelAnimator; // 3D模型的动画控制器
    public string specificActionStateName; // 特定动作的动画状态名称

    private int specificActionStateHash; // 特定动作状态的哈希值

    private void Start()
    {
        specificActionStateHash = Animator.StringToHash(specificActionStateName);
    }

    public void StartSpecificAction()
    {
        modelAnimator.CrossFade(specificActionStateHash, 0.1f);
    }
}
