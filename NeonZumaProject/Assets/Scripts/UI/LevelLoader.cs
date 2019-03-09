using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class LevelLoader : MonoBehaviour
    {
        public void LoadLevel(int id)
        {
            SceneManager.LoadScene(id);
        }
    }
}
