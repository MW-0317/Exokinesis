using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using StarterAssets;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using TMPro;
#pragma warning disable CS0618 // Type or member is obsolete

namespace SlimUI.ModernMenu{
	public class UISettingsManager : MonoBehaviour
	{
		public enum Platform {Desktop, Mobile};
		public Platform platform;
		
		private Resolution[] _resolutions;
		private int _resolutionIndex;
		
		private List<DisplayInfo> _displays = new List<DisplayInfo>();

		[Header("VIDEO SETTINGS")]
		public UniversalRenderPipelineAsset urpAsset;
		public TMP_Dropdown resolutionDropdown;
		public TMP_Dropdown displayDropdown;
		public TMP_Dropdown monitorDropdown;
		public TMP_Dropdown vSyncDropdown;
		public TMP_Dropdown frameLimitDropdown;

		[Header("CONTROLS SETTINGS")]
		public TMP_Text mousexSensitivityText;
		public TMP_Text mouseySensitivityText;
		public TMP_Text controllerxSensitivityText;
		public TMP_Text controllerySensitivityText;
		public GameObject mouseSensitivityXSlider;
		public GameObject mouseSensitivityYSlider;
		public GameObject controllerSensitivityXSlider;
		public GameObject controllerSensitivityYSlider;

		[Header("GRAPHICS SETTINGS")]
		public TMP_Dropdown qualityDropdown;
		public TMP_Dropdown aaDropdown;
			
		[Header("GAME SETTINGS")]
		public AudioMixer audioMixer;
		public GameObject masterSlider;
		public GameObject musicSlider;
		public GameObject sfxSlider;

		public void Start ()
		{
			Application.targetFrameRate = Screen.currentResolution.refreshRate;
			
			// Add available resolutions to resolution dropdown, excluding duplicates
			PopulateResolutions();
			PopulateMonitors();

			// check slider values
			//sensitivityXSlider.GetComponent<Slider>().value = PlayerPrefs.GetFloat("XSensitivity");
			//sensitivityYSlider.GetComponent<Slider>().value = PlayerPrefs.GetFloat("YSensitivity");
			
			LoadSettings();
		}

		public void SetMaster(float masterVolume)
		{
			float volume = LinearToDecibel(masterVolume);
			audioMixer.SetFloat("Master", volume);
			PlayerPrefs.SetFloat("MasterVolume", masterVolume);
		}

		public void SetMusic (float musicVolume)
		{
			float volume = LinearToDecibel(musicVolume);
			audioMixer.SetFloat("Music", volume);
			PlayerPrefs.SetFloat("MusicVolume", musicVolume);
		}

		public void SetSfx (float sfxVolume)
		{
			float volume = LinearToDecibel(sfxVolume);
			audioMixer.SetFloat("SFX", volume);
			PlayerPrefs.SetFloat("SfxVolume", sfxVolume);
		}

		public void SetMouseXSensitivity (float xSensitivity)
		{
			PlayerPrefs.SetFloat("MouseXSensitivity", xSensitivity);
			mousexSensitivityText.text = xSensitivity.ToString("F1");

			var controller = FindObjectOfType<ThirdPersonController>();
			if (controller != null)
			{
				controller.mouseXSensitivity = xSensitivity;
			}
		}

		public void SetMouseYSensitivity (float ySensitivity)
		{
			PlayerPrefs.SetFloat("MouseYSensitivity", ySensitivity);
			mouseySensitivityText.text = ySensitivity.ToString("F1");
			
			var controller = FindObjectOfType<ThirdPersonController>();
			if (controller != null)
			{
				controller.mouseYSensitivity = ySensitivity;
			}
		}
		
		public void SetControllerXSensitivity (float xSensitivity)
		{
			PlayerPrefs.SetFloat("ControllerXSensitivity", xSensitivity);
			controllerxSensitivityText.text = xSensitivity.ToString("F1");
			
			var controller = FindObjectOfType<ThirdPersonController>();
			if (controller != null)
			{
				controller.controllerXSensitivity = xSensitivity;
			}
		}

		public void SetControllerYSensitivity (float ySensitivity)
		{
			PlayerPrefs.SetFloat("ControllerYSensitivity", ySensitivity);
			controllerySensitivityText.text = ySensitivity.ToString("F1");
			
			var controller = FindObjectOfType<ThirdPersonController>();
			if (controller != null)
			{
				controller.controllerYSensitivity = ySensitivity;
			}
		}

		private void PopulateResolutions()
		{
			_resolutions = Screen.resolutions.Select(resolution => new Resolution { width = resolution.width, height = resolution.height })
				.Distinct(new ResolutionComparer())
				.OrderBy(resolution => resolution.width)
				.ThenBy(resolution => resolution.height)
				.ToArray();

			List<string> options = new List<string>();

			int currentResolutionIndex = 0;
			for (int i = 0; i < _resolutions.Length; i++)
			{
				string option = _resolutions[i].width + "x" + _resolutions[i].height;
				options.Add(option);
        
				if (_resolutions[i].width == Screen.currentResolution.width && _resolutions[i].height == Screen.currentResolution.height)
				{
					currentResolutionIndex = i;
				}
			}

			resolutionDropdown.ClearOptions();
			resolutionDropdown.AddOptions(options);
			resolutionDropdown.value = currentResolutionIndex;
			resolutionDropdown.RefreshShownValue();
			
			PopulateFrameLimits();

			_resolutionIndex = currentResolutionIndex;
		}

		private void PopulateMonitors()
		{
			// Clear existing entries
			_displays.Clear();
			List<string> monitorOptions = new List<string>();

			// Fill the list with DisplayInfo objects
			Screen.GetDisplayLayout(_displays);

			int currentMonitorIndex = _displays.IndexOf(Screen.mainWindowDisplayInfo);

			for (int i = 0; i < _displays.Count; i++)
			{
				// You might want to create a more user-friendly name based on the display properties
				string displayName = "Monitor " + (i + 1) + " (" + _displays[i].width + "x" + _displays[i].height + ")";
				monitorOptions.Add(displayName);
			}

			monitorDropdown.ClearOptions();
			monitorDropdown.AddOptions(monitorOptions);
			monitorDropdown.value = currentMonitorIndex;
			monitorDropdown.RefreshShownValue();
		}

		private void PopulateFrameLimits()
		{
			Resolution selectedResolution = _resolutions[_resolutionIndex];
			var matchingRefreshRates = Screen.resolutions
				.Where(res => res.width == selectedResolution.width && res.height == selectedResolution.height)
				.Select(res => res.refreshRate)
				.Distinct()
				.OrderBy(rate => rate)
				.ToList();

			frameLimitDropdown.ClearOptions();
			frameLimitDropdown.AddOptions(matchingRefreshRates.ConvertAll(rate => rate.ToString() + " FPS"));
			
			frameLimitDropdown.options.Insert(0, new TMP_Dropdown.OptionData("None"));
			// Set the dropdown to the current or a default refresh rate
			frameLimitDropdown.value = matchingRefreshRates.IndexOf(Screen.currentResolution.refreshRate);
			frameLimitDropdown.RefreshShownValue();
		}

		public void SetResolution(int resolutionIndex)
		{
			Resolution resolution = _resolutions[resolutionIndex];
			
			Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
			
			PlayerPrefs.SetInt("ResolutionIndex", resolutionIndex);

			_resolutionIndex = resolutionIndex;
			
			PopulateFrameLimits();
		}

		// public void SetFrameLimit(int frameLimitIndex)
		// {
		// 	if (frameLimitIndex <= 0) { // "None" option or default selection
		// 		Application.targetFrameRate = -1; // No frame rate cap
		// 	} else {
		// 		// Adjust index to account for the "None" option
		// 		int adjustedIndex = frameLimitIndex - 1;
		//
		// 		var selectedResolution = _resolutions[_resolutionIndex];
		// 		var refreshRates = Screen.resolutions
		// 			.Where(res => res.width == selectedResolution.width && res.height == selectedResolution.height)
		// 			.Select(res => res.refreshRate)
		// 			.Distinct()
		// 			.OrderBy(rate => rate)
		// 			.ToArray();
		//
		// 		if (adjustedIndex < refreshRates.Length) {
		// 			int selectedFrameRate = refreshRates[adjustedIndex];
		// 			Application.targetFrameRate = selectedFrameRate;
		// 		} else {
		// 			Debug.LogError("Invalid frame rate option selected.");
		// 		}
		// 	}
		// 	PlayerPrefs.SetInt("FrameLimitIndex", frameLimitIndex);
		// }

		public void SetVSync(int vSyncIndex)
		{
			switch (vSyncIndex)
			{
				case 0: // Off
					QualitySettings.vSyncCount = 0;
					break;
				case 1: // Every V Blank
					QualitySettings.vSyncCount = 1;
					break;
				case 2: // Every Second V Blank
					QualitySettings.vSyncCount = 2;
					break;
				default:
					break;
			}
			PlayerPrefs.SetInt("VSyncIndex", vSyncIndex);
		}

		public void SetQuality(int qualityIndex)
		{
			QualitySettings.SetQualityLevel(qualityIndex);
			
			PlayerPrefs.SetInt("QualityIndex", qualityIndex);
		}

		public void SetDisplay(int displayIndex)
		{
			switch (displayIndex)
			{
				case 0:
					Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
					break;
				case 1:
					Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
					break;
				case 2:
					Screen.fullScreenMode = FullScreenMode.Windowed;
					break;
				default:
					break;
			}
			PlayerPrefs.SetInt("DisplayIndex", displayIndex);
		}

		public void SetMonitor(int monitorIndex)
		{
			if (monitorIndex >= 0 && monitorIndex < _displays.Count)
			{
				DisplayInfo selectedDisplay = _displays[monitorIndex];
				Vector2Int position = new Vector2Int(0, 0);

				Screen.MoveMainWindowTo(selectedDisplay, position);
			}
			PlayerPrefs.SetInt("MonitorIndex", monitorIndex);
		}

		public void SetAntiAliasing(int aaIndex)
		{
			if (urpAsset == null)
			{
				Debug.LogWarning("Universal Render Pipeline Asset is not assigned.");
				return;
			}

			switch (aaIndex)
			{
				case 0: // No Anti-Aliasing
					DisableTaa();
					urpAsset.msaaSampleCount = 1;
					break;
				case 1: // 2x MSAA
					DisableTaa();
					urpAsset.msaaSampleCount = 2;
					break;
				case 2: // 4x MSAA
					DisableTaa();
					urpAsset.msaaSampleCount = 4;
					break;
				case 3: // 8x MSAA
					DisableTaa();
					urpAsset.msaaSampleCount = 8;
					break;
				case 4: // TAA (Low)
					urpAsset.msaaSampleCount = 1; // Disable MSAA
					EnableTaa(3);
					break;
				case 5: // TAA (Medium)
					urpAsset.msaaSampleCount = 1;
					EnableTaa(2);
					break;
				case 6: // TAA (High)
					urpAsset.msaaSampleCount = 1;
					EnableTaa(1);
					break;
				default:
					Debug.LogWarning("Unsupported Anti-Aliasing index.");
					break;
			}
			SynchronizeMsaaSettings();
			
			PlayerPrefs.SetInt("AAIndex", aaIndex);
		}
		
		private void SynchronizeMsaaSettings()
		{
			if (urpAsset == null)
			{
				Debug.LogError("URP Asset is not assigned.");
				return;
			}
			
			var currentUrpAsset = GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset;
			if (currentUrpAsset != null && currentUrpAsset != urpAsset)
			{
				currentUrpAsset.msaaSampleCount = urpAsset.msaaSampleCount;
			}
		}

		private void DisableTaa()
		{
			foreach (var cam in Camera.allCameras)
			{
				if (cam.TryGetComponent<UniversalAdditionalCameraData>(out var cameraData))
				{
					cameraData.antialiasing = AntialiasingMode.None;
				}
			}
		}

		private void EnableTaa(int quality)
		{
			foreach (var cam in Camera.allCameras)
			{
				if (cam.TryGetComponent<UniversalAdditionalCameraData>(out var cameraData))
				{
					cameraData.antialiasing = AntialiasingMode.TemporalAntiAliasing;
					if (quality == 1)
						cameraData.antialiasingQuality = AntialiasingQuality.High;
					if (quality == 2)
						cameraData.antialiasingQuality = AntialiasingQuality.Medium;
					if (quality == 3)
						cameraData.antialiasingQuality = AntialiasingQuality.Low;
				}
			}
		}

		public void LoadSettings()
		{
			// Volume settings
			float masterVolume = PlayerPrefs.GetFloat("MasterVolume", 0.75f);
			masterSlider.GetComponent<Slider>().value = masterVolume; // Set slider
			SetMaster(masterVolume); // Set mixer
			
			float musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.7f);
			musicSlider.GetComponent<Slider>().value = musicVolume;
			SetMusic(musicVolume);
			
			float sfxVolume = PlayerPrefs.GetFloat("SfxVolume", 1f);
			sfxSlider.GetComponent<Slider>().value = sfxVolume;
			SetSfx(sfxVolume);
			
			// Control settings
			float mousexSensitivity = PlayerPrefs.GetFloat("MouseXSensitivity", 1f);
			mouseSensitivityXSlider.GetComponent<Slider>().value = mousexSensitivity;
			mousexSensitivityText.text = mousexSensitivity.ToString("F1");
			SetMouseXSensitivity(mousexSensitivity);
			
			float mouseySensitivity = PlayerPrefs.GetFloat("MouseYSensitivity", 1f);
			mouseSensitivityYSlider.GetComponent<Slider>().value = mouseySensitivity;
			mouseySensitivityText.text = mouseySensitivity.ToString("F1");
			SetMouseYSensitivity(mouseySensitivity);
			
			float controllerxSensitivity = PlayerPrefs.GetFloat("ControllerXSensitivity", 0.5f);
			controllerSensitivityXSlider.GetComponent<Slider>().value = controllerxSensitivity;
			controllerxSensitivityText.text = controllerxSensitivity.ToString("F1");
			SetControllerXSensitivity(controllerxSensitivity);
			
			float controllerySensitivity = PlayerPrefs.GetFloat("ControllerYSensitivity", 0.5f);
			controllerSensitivityYSlider.GetComponent<Slider>().value = controllerySensitivity;
			controllerySensitivityText.text = controllerySensitivity.ToString("F1");
			SetControllerYSensitivity(controllerySensitivity);
			
			// Video settings
			int displayIndex = PlayerPrefs.GetInt("DisplayIndex", 1);
			displayDropdown.value = displayIndex;
			displayDropdown.RefreshShownValue();
			SetDisplay(displayIndex);

			int defaultResolutionIndex = _resolutions.Length - 1; // Default is highest resolution
			int resolutionIndex = PlayerPrefs.GetInt("ResolutionIndex", defaultResolutionIndex);
			resolutionDropdown.value = resolutionIndex;
			resolutionDropdown.RefreshShownValue();
			SetResolution(resolutionIndex);

			int monitorIndex = PlayerPrefs.GetInt("MonitorIndex", 0);
			monitorDropdown.value = monitorIndex;
			monitorDropdown.RefreshShownValue();
			SetMonitor(monitorIndex);

			int vsyncIndex = PlayerPrefs.GetInt("VSyncIndex", 1);
			vSyncDropdown.value = vsyncIndex;
			vSyncDropdown.RefreshShownValue();
			SetVSync(vsyncIndex);

			// int frameLimitIndex = PlayerPrefs.GetInt("FrameLimitIndex", 0);
			// frameLimitDropdown.value = frameLimitIndex;
			// frameLimitDropdown.RefreshShownValue();
			// SetFrameLimit(frameLimitIndex);
			
			// Graphics settings
			int qualityIndex = PlayerPrefs.GetInt("QualityIndex", 4);
			qualityDropdown.value = qualityIndex;
			qualityDropdown.RefreshShownValue();
			SetQuality(qualityIndex);
			
			int aaIndex = PlayerPrefs.GetInt("AAIndex", 6);
			aaDropdown.value = aaIndex;
			aaDropdown.RefreshShownValue();
			SetAntiAliasing(aaIndex);
		}

		public void ResetPlayerPrefs()
		{
			PlayerPrefs.DeleteAll();
			PlayerPrefs.Save();
			LoadSettings();
		}

		float LinearToDecibel(float linear)
		{
			if (linear <= 0) return -80f;
			float dB = 20.0f * Mathf.Log10(linear); // Calculate dB
			return Mathf.Max(dB, -80f); // Clamp to -80 dB
		}

		private class ResolutionComparer : IEqualityComparer<Resolution>
		{
			public bool Equals(Resolution x, Resolution y)
			{
				return x.width == y.width && x.height == y.height;
			}

			public int GetHashCode(Resolution obj)
			{
				return obj.width.GetHashCode() ^ obj.height.GetHashCode();
			}
		}
	}
}