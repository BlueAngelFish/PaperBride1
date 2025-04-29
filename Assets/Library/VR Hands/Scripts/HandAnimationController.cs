using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class HandAnimationController : MonoBehaviour {
	//public List<GameObject> controllerPrefabs;
	public InputDeviceCharacteristics controllerType;
	InputDevice targetDevice;

	Animator animatorController;
	bool isControllerFound;

	void Start() {
		animatorController = GetComponent<Animator>();
		Initialize();
	}

	void Initialize() {
		List<InputDevice> devices = new List<InputDevice>();
		InputDevices.GetDevicesWithCharacteristics(controllerType, devices);

		//foreach(var device in devices) {
		//	Debug.Log(device.name + " " + device.characteristics);
		//}

		if(devices.Count.Equals(0)) {
			Debug.Log("No XR device found");
		} else {
			targetDevice = devices[0];
			//GameObject devicePrefab = controllerPrefabs.Find(controller => controller.name == targetDevice.name);

			//if(devicePrefab) {
			//	Instantiate(devicePrefab, transform);
			//} else {
			//	Debug.LogError("Unable to find corresponding controller device model");
			//	Instantiate(controllerPrefabs[0], transform); // Use a default controller
			//}

			isControllerFound = true;
		}
	}

	void Update() {
		if(!isControllerFound) {
			Initialize();
		} else {
			if(targetDevice.TryGetFeatureValue(CommonUsages.primaryButton, out bool primaryButtonValue)) {
				if(primaryButtonValue)
					Debug.Log("Primary button " + primaryButtonValue);
			}

			if(targetDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 primary2DAxisValue)) {
				if(primary2DAxisValue != Vector2.zero)
					Debug.Log("Primary touchpad " + primary2DAxisValue);
			}

			if(targetDevice.TryGetFeatureValue(CommonUsages.trigger, out float triggerValue)) {
				animatorController.SetFloat("Trigger", triggerValue);
			}

			if(targetDevice.TryGetFeatureValue(CommonUsages.grip, out float gripValue)) {
				animatorController.SetFloat("Grip", gripValue);
			}
		}
	}
}