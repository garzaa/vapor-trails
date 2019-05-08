using UnityEngine;
using UnityEngine.SceneManagement;

public class HeadspaceController : MonoBehaviour {
    Vector2 lastPlayerPos;
    Animator animator;
    Animator playerAnimator;
    string lastScene;
    bool toFinishLeavingHeadspace = false;

    void Start() {
        SceneManager.sceneLoaded += OnSceneLoaded;
        animator = GetComponent<Animator>();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        if (lastScene != null && scene.name != "Headspace" && toFinishLeavingHeadspace) {
            FinishLeavingHeadspace();
        } else if (scene.name == "Headspace") {
            GlobalController.pc.GetComponent<Animator>().SetTrigger("InstantKneel");
        } else {
            // catch any edge cases
            toFinishLeavingHeadspace = false;
        }
    }

    public void StartEnterHeadspaceAnimation() {
        lastScene = SceneManager.GetActiveScene().name;
        lastPlayerPos = GlobalController.pc.transform.position;
        // animator.SetTrigger("EnterHeadspace");
    }

    public void AnimationEnterHeadspace() {
        GlobalController.LoadScene("Headspace/Headspace", Beacon.A);
    }

    public void LeaveHeadspace() {
        toFinishLeavingHeadspace = true;
        SceneManager.LoadScene(lastScene);
    }

    public void FinishLeavingHeadspace() {
        lastScene = null;
        toFinishLeavingHeadspace = false;
        GlobalController.MovePlayerTo(lastPlayerPos);
        GlobalController.pc.GetComponent<Animator>().SetTrigger("InstantKneel");
        animator.SetTrigger("LeaveHeadspace");
    }
}
