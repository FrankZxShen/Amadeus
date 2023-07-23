using UnityEngine;
using System.Collections;

public class DancingControl : MonoBehaviour
{
    public Animator modelAnimator; // 3D模型的动画控制器
    public string specificActionStateName; // 特定动作的动画状态名称
    public string IdleStateName;//待机动画名称
    public AudioSource backgroundMusicAudioSource; // 背景音乐的AudioSource组件

    public AudioSource danceMusic; // 特定跳舞时播放的音乐

    public bool isDancing = false; // 标记是否正在跳舞
    public float danceDuration;//动画持续时间
    public void PlayDanceAnimation()
    {
        // 如果正在跳舞，则直接返回，避免重复播放
        if (isDancing)
            return;

        // 暂停背景音乐
        backgroundMusicAudioSource.Pause();

        // 播放特定音乐
        danceMusic.Play();

        // 播放特定动作的舞蹈动画
        modelAnimator.Play(specificActionStateName);
        
        // 假设你的动画持续时间是 danceDuration，根据实际情况设置等待时间
        // danceDuration = modelAnimator.GetCurrentAnimatorStateInfo(0).length;
        
        // 标记正在跳舞
        isDancing = true;

        // 在跳舞完成后，调用 StopDancing 方法
        
        // Invoke("StopDancing", danceDuration);
      
        // 在跳舞完成后，调用 StopDancing 方法
        StartCoroutine(WaitForDanceComplete());
    }

    private IEnumerator WaitForDanceComplete()
    {
    // 等待动画播放完成
    yield return new WaitForSeconds(danceDuration);

    // 停止跳舞动画
    modelAnimator.Play(IdleStateName); // 将 "Idle" 替换为你的待机动画状态名称

    // 停止特定音乐
    danceMusic.Stop();

    // 恢复背景音乐的播放
    backgroundMusicAudioSource.Play();

    // 标记跳舞完成
    isDancing = false;
    }
    
    // private float GetAnimationDuration(string animationStateName)
    // {
    //     float duration = 0f;
    //     RuntimeAnimatorController ac = modelAnimator.runtimeAnimatorController;
    //     for (int i = 0; i < ac.animationClips.Length; i++)
    //     {
    //         if (ac.animationClips[i].name == animationStateName)
    //         {
    //             duration = ac.animationClips[i].length;
    //             break;
    //         }
    //     }
    //     return duration;
    // }

    public void StopDancing()
    {
        // 如果当前没有在跳舞，则直接返回
        if (!isDancing)
            return;

        // 停止跳舞动画
        modelAnimator.Play(IdleStateName); // 将 "Idle" 替换为你的待机动画状态名称

        // 停止特定音乐
        danceMusic.Stop();

        // 恢复背景音乐的播放
        backgroundMusicAudioSource.Play();

        // 标记跳舞完成
        isDancing = false;
    }


    // 其他DancingControl脚本中的逻辑代码
}
