using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour {

    public PlayerHealth playerHealth;
    public float restartDelay = 2f;

    Animator anim;
    float restartTimer;

    void Awake()
    {
        anim = GetComponent<Animator>();
    }
	
	// Update is called once per frame
	void Update () {
		if (playerHealth.currentHealth <= 0)
        {
            anim.SetTrigger("GameOver");
            restartTimer += Time.deltaTime;

            if (restartTimer >= restartDelay)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
	}
}
