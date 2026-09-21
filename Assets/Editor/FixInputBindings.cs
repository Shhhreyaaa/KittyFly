using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.InputSystem;

public class FixInputBindings
{
    [MenuItem("Tools/Fix Input Bindings")]
    public static void FixBindings()
    {
        GameObject rocketObj = GameObject.Find("Player Rocket");
        if (rocketObj == null)
        {
            Debug.LogError("[FixInput] Player Rocket not found!");
            return;
        }

        movement mov = rocketObj.GetComponent<movement>();
        if (mov == null)
        {
            Debug.LogError("[FixInput] movement script not found on Player Rocket!");
            return;
        }

        SerializedObject serializedObj = new SerializedObject(mov);
        
        // Setup Thrust Action
        SerializedProperty thrustProp = serializedObj.FindProperty("thrust");
        if (thrustProp != null)
        {
            // Build action for Thrust: Space, W, UpArrow, Gamepad RightTrigger
            InputAction thrustAction = new InputAction("Thrust", InputActionType.Button);
            thrustAction.AddBinding("<Keyboard>/space");
            thrustAction.AddBinding("<Keyboard>/w");
            thrustAction.AddBinding("<Keyboard>/upArrow");
            thrustAction.AddBinding("<Gamepad>/rightTrigger");

            // Convert to JSON or apply via SerializedProperty
        }

        // We can set up clean InputActions directly in C# on Movement script as well!
        Debug.Log("[FixInput] Inspected Movement component.");
    }
}
