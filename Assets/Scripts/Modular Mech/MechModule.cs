using UnityEngine;

public class MechModule : MonoBehaviour {
    public MechType origin = MechType.Karasu;
    public MechModuleType mechModuleType = MechModuleType.Torso;

    private MechModuleJoint[] mechModuleJoints;
 
    public void Start() {
        GetMechModuleJoints();

        for (int i = 0; i < mechModuleJoints.Length; i++) {
            Debug.Log(mechModuleJoints[i].name);
        }
    }

    public void GetMechModuleJoints() {
        mechModuleJoints = GetComponentsInChildren<MechModuleJoint>();
    }
}
