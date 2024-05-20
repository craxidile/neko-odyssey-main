using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace DataPersistence
{
    public class SaveSlotCheckData : MonoBehaviour
    {
        public int slotIndex;
        public TextMeshProUGUI textui;

        private void OnEnable()
        {
            string checkSaveSlot = DataPersistenceManager.instance.CheckSaveSlot(slotIndex) ? "Found save " + slotIndex : "--/--/--";

            textui.text = "Slot " + slotIndex + " " + checkSaveSlot;
        }
    }
}