using UnityEngine;
using DaggerfallWorkshop.Game.Utility.ModSupport; // Required for mod support
using DaggerfallWorkshop.Game.Utility.ModSupport.ModSettings;
using DaggerfallWorkshop.Game.Entity; // Required for EntityEffectManager
using DaggerfallWorkshop.Game.MagicAndEffects;
using DaggerfallWorkshop.Game.UserInterfaceWindows;
using DaggerfallWorkshop.Game; // Required for GameManager

public class AlertPoisonDiseaseMod : MonoBehaviour
{
    public static Mod Mod;

    // Polling interval in seconds
    private float pollingInterval = 1.0f;
    private float timeSinceLastCheck = 0.0f;
    private bool alreadyShowMessageDisease = false;
    private bool alreadyShowMessagePoison = false;
    public static bool enablePoisonMessage { get; set; }
    public static bool enableDiseaseMessage { get; set; }
    public static string poisonMessage { get; set; }
    public static string diseaseMessage { get; set; }


    [Invoke(StateManager.StateTypes.Start, 0)]
    public static void Init(InitParams initParams)
    {
        // Store the mod reference
        Mod = initParams.Mod;
 
        // Create a new GameObject to hold the mod component
        GameObject go = new GameObject(Mod.Title);
        go.AddComponent<AlertPoisonDiseaseMod>();

        // Allow to access mod settings during gameplay 
        Mod.LoadSettingsCallback = LoadSettings;

        Mod.IsReady = true;
    }

    private void Start()
    {
        Mod.LoadSettings();
    }

    void Update()
    {
        timeSinceLastCheck += Time.deltaTime;
        if (timeSinceLastCheck >= pollingInterval)
        {
            CheckPlayer();
            timeSinceLastCheck = 0.0f; // Reset the timer
        }
    }
    static void LoadSettings(ModSettings modSettings, ModSettingsChange change)
    {
        ModSettings settings = Mod.GetSettings();
        if (settings != null)
        {
            enablePoisonMessage = settings.GetValue<bool>("Settings", "EnablePoisonMessage");
            enableDiseaseMessage = settings.GetValue<bool>("Settings", "EnableDiseaseMessage");
            poisonMessage = settings.GetValue<string>("Settings", "PoisonMessage");
            diseaseMessage = settings.GetValue<string>("Settings", "DiseaseMessage");
        }
    }

    void CheckPlayer()
    {
        EntityEffectManager effectManager = GameManager.Instance.PlayerEffectManager;
        bool hasPoison = effectManager.PoisonCount > 0;
        bool hasDisease = effectManager.DiseaseCount > 0;

        if (enablePoisonMessage && !alreadyShowMessagePoison && hasPoison)
        {
            DaggerfallUI.MessageBox(poisonMessage);
            alreadyShowMessagePoison = true;
        }
        if (enableDiseaseMessage && !alreadyShowMessageDisease && hasDisease)
        {
            DaggerfallUI.MessageBox(diseaseMessage);
            alreadyShowMessageDisease = true;
        }

        if (!hasPoison)
        {
            alreadyShowMessagePoison = false;
        }
        if (!hasDisease)
        {
            alreadyShowMessageDisease = false;
        }
    }
}
