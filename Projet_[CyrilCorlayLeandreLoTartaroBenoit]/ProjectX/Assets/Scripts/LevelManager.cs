using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public float restartDelay = 3f;

    public int TotalZombiesKilled = 0;
    public int TotalZombies = 12;

    public GameObject VictoryUI;
    public GameObject DefeatedUI;
    public Player player;


    public bool WinCondition()
    {
        return TotalZombiesKilled >= TotalZombies;
    }
    public bool LostCondition()
    {
        return player.health <= 0;
    }

    public void CompleteLevel()
    {
        player.enabled = false;
        VictoryUI.SetActive(true);
        Invoke("MenuLvl", restartDelay);
    }

    public void FailLevel()
    {
        DefeatedUI.SetActive(true);
        Invoke("MenuLvl", restartDelay);
    }

    private void MenuLvl()
    {
        SceneManager.LoadScene("Menu");
    }
    // Update is called once per frame
    void Update()
    {
        if (WinCondition())
        {
            CompleteLevel();
        }
        else if (LostCondition())
        {
            FailLevel();
        }

    }
}