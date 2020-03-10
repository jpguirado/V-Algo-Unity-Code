using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuAbout : MonoBehaviour
{
    public string VAlgoCode, VAlgoBuilds, jpguiradoGithub;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Open V-Algo Code Github Proyect
    public void OpenVAlgoCode()
    {
        Application.OpenURL(VAlgoCode);
    }

    //Open V-Algo Builds Github Proyect
    public void OpenVAlgoBuilds()
    {
        Application.OpenURL(VAlgoBuilds);
    }

    //Open jpuirado github profile
    public void OpenjpguiradoGuthub()
    {
        Application.OpenURL(jpguiradoGithub);
    }
}
