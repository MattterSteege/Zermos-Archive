using TMPro;
using UnityEngine;

public class AppointmentInfo : MonoBehaviour
{
    public void SetAppointmentInfo(string _lokaal = "", string _tijd = null, string _vak = null, string _lesUur = null, Schedule.Appointment _appointment = null)
    {
        if (vak != null)
            vak.text = _vak;
        if(tijdLokaal != null)
            tijdLokaal.text = "Lokaal " + _lokaal + " // " + _tijd;
        if(lesUur != null)
            lesUur.text = _lesUur;

        if (kwtUurAvailable != null)
            kwtUurAvailable.SetActive(_appointment?.actions?.Count > 0 && _appointment.actions[0]?.allowed == true);

        if (_appointment != null)
            Appointment = _appointment;
        
    }

    [SerializeField] TMP_Text vak;
    [SerializeField] TMP_Text tijdLokaal;
    [SerializeField] TMP_Text lesUur;
    [SerializeField] GameObject kwtUurAvailable;
    public Schedule.Appointment Appointment;

    public void hide()
    {
        this.gameObject.SetActive(false);
    }
}