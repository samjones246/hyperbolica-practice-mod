using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MelonLoader;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Hyperbolica_Practice_Mod
{
    public class Main : MelonMod
    {
        Player player;
        List<string> playableScenes;
        Text overlay;
        float baseSpeed;
        int speedMult = 1;
        bool noclip = false;
        bool overlayInitialised = false;
        public override void OnApplicationStart()
        {
            playableScenes = new List<string>
            {
                "Over",
                "Cafe",
                "Farm",
                "Snow",
                "Maze",
                "Gallery",
                "Glitch"
            };
        }
        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            if (!overlayInitialised)
            {
                InitOverlay();
            }
            GameObject pgo = GameObject.Find("HyperbolicaPlayer");
            if (pgo == null)
            {
                return;
            }
            player = pgo.GetComponent<Player>();
            baseSpeed = player.walkingSpeed;
        }

        public void InitOverlay()
        {
            LoggerInstance.Msg("Initialising Overlay...");
            LoggerInstance.Msg("Creating canvas...");
            GameObject eventQueue = GameObject.Find("EventQueue");
            GameObject pauseCanvas = null;
            GameObject overlayObject = null;
            foreach(Transform child in eventQueue.GetComponentsInChildren<Transform>(true))
            {
                if (child.name == "PauseCanvas")
                {
                    pauseCanvas = child.gameObject;
                    break;
                }
            }
            GameObject goCanvas = GameObject.Instantiate(pauseCanvas, GameObject.Find("EventQueue").transform);
            GameObject.Destroy(goCanvas.GetComponent<VerticalLayoutGroup>());
            goCanvas.name = "OverlayCanvas";
            goCanvas.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            goCanvas.SetActive(true);
            for (int i=0;i<goCanvas.transform.childCount;i++)
            {
                Transform child = goCanvas.transform.GetChild(i);
                if (child.name == "Resume")
                {
                    overlayObject = child.GetChild(0).gameObject;
                    overlayObject.transform.parent = null;
                }
                LoggerInstance.Msg("Destroying " + child.name);
                GameObject.Destroy(child.gameObject);
            }
            LoggerInstance.Msg("Configuring Text Container...");
            overlayObject.transform.parent = goCanvas.transform;
            GameObject.Destroy(overlayObject.GetComponent<Button>());
            GameObject.Destroy(overlayObject.GetComponent<Image>());
            overlay = overlayObject.GetComponentInChildren<Text>();
            overlay.alignment = TextAnchor.UpperLeft;
            RectTransform component = overlayObject.GetComponent<RectTransform>();
            component.sizeDelta = new Vector2((float)(Screen.currentResolution.width / 3), (float)(Screen.currentResolution.height / 3));
            component.pivot = new Vector2(0f, 1f);
            component.anchorMin = new Vector2(0f, 1f);
            component.anchorMax = new Vector2(0f, 1f);
            component.anchoredPosition = new Vector2(15f, -15f);
            overlay.text = "";
            overlay.fontSize = 20;
            overlayInitialised = true;
        }

        public void UpdateOverlay()
        {
            string newText = "";
            if (playableScenes.Contains(SceneManager.GetActiveScene().name)){
                newText += $"Scene: {SceneManager.GetActiveScene().name}\n";
                newText += $"Speed: {speedMult}x\n";
                newText += $"Noclip: {(noclip ? "ON" : "OFF")}";
                overlay.text = newText;
            }
            else
            {
                overlay.text = "";
            }
        }

        public override void OnUpdate()
        {
            UpdateOverlay();
            if (SceneManager.GetActiveScene().name == "MainMenu")
            {
                return;
            }

            for (int i = 0; i < playableScenes.Count; i++)
            {
                if (Input.GetKeyDown((KeyCode)i + 49))
                {
                    SceneManager.LoadScene(playableScenes[i]);
                }
            }

            if (Input.GetKeyDown(KeyCode.LeftBracket))
            {
                speedMult = Mathf.Max(1, speedMult - 1);
            }
            if (Input.GetKeyDown(KeyCode.RightBracket))
            {
                speedMult++;
            }
            player.walkingSpeed = baseSpeed * speedMult;

            if (Input.GetKeyDown(KeyCode.K))
            {
                player.ignoreColliders = !player.ignoreColliders;
                player.freeFly = !player.freeFly;
                noclip = !noclip;
            }
        }
    }
}
