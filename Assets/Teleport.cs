using UnityEngine;

using UnityEngine.InputSystem;

public class Teleporter : MonoBehaviour
{
    public UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation.TeleportationProvider provider;
    public UnityEngine.XR.Interaction.Toolkit.Interactors.XRRayInteractor rayInteractor;
    public InputActionProperty teleportAction;

    void Update()
    {
        // We check for "WasReleased" to ensure the teleport happens 
        // only when the user finishes their intended gesture.
        if (teleportAction.action.WasReleasedThisFrame())
        {
            RequestTeleport();
        }
    }

    private void RequestTeleport()
    {
        // The core logic: we sample the environment and validate 
        // that we hit a valid surface before instructing the provider.
        if (rayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit hit))
        {
            UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation.TeleportRequest request = new UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation.TeleportRequest()
            {
                destinationPosition = hit.point,
                matchOrientation = UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation.MatchOrientation.TargetUpAndForward
            };

            provider.QueueTeleportRequest(request);
        }
    }
}
