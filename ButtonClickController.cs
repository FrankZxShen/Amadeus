using UnityEngine;

public class ButtonClickController : MonoBehaviour
{
    public Animator modelAnimator; // 3D模型的动画控制器
    public string specificActionStateName; // 特定动作的动画状态名称

    private int specificActionStateHash; // 特定动作状态的哈希值
    public DancingControl dancingControl; // 引用 DancingControl 脚本
    private void Start()
    {
        // 初始化 specificActionStateHash
        specificActionStateHash = Animator.StringToHash(specificActionStateName);
    }

    public void StartSpecificAction()
    {
         // 确保 dancingControl 不为空
        if (dancingControl == null)
        {
            Debug.LogError("dancingControl is not assigned in ButtonClickController!");
            return;
        }
        // 在这里检查 DancingControl 是否正在播放舞蹈动画
        if (!dancingControl.isDancing)
        {
            modelAnimator.CrossFade(specificActionStateHash, 0.1f);
        }
    }
}
