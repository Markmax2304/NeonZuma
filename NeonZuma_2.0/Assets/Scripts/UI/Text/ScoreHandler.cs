using UnityEngine;
using UnityEngine.UI;

public class ScoreHandler : MonoBehaviour
{
    [SerializeField]
    private Text playerScore;
    [SerializeField]
    private Text botScore;

    private void Start()
    {
        Contexts.sharedInstance.global.SetScoreHandler(playerScore, botScore);
    }
}
