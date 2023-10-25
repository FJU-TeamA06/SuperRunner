using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KBInputViewScript : MonoBehaviour
{
    // Start is called before the first frame update

    public RawImage ObjectAImage;
    public Texture2D Texture_KB_A;
    public Texture2D Texture_KB_A_Pressed;
    public RawImage ObjectSImage;
    public Texture2D Texture_KB_S;
    public Texture2D Texture_KB_S_Pressed;
    public RawImage ObjectWImage;
    public Texture2D Texture_KB_W;
    public Texture2D Texture_KB_W_Pressed;
    public RawImage ObjectDImage;
    public Texture2D Texture_KB_D;
    public Texture2D Texture_KB_D_Pressed;
    public RawImage ObjectSpaceImage;
    public Texture2D Texture_KB_Space; 
    public Texture2D Texture_KB_Space_Pressed;
    public RawImage ObjectEnterImage;
    public Texture2D Texture_KB_Enter;
    public Texture2D Texture_KB_Enter_Pressed;
    public RawImage ObjectCImage;
    public Texture2D Texture_KB_C;
    public Texture2D Texture_KB_C_Pressed;
    public RawImage ObjectRImage;
    public Texture2D Texture_KB_R;
    public Texture2D Texture_KB_R_Pressed; 
    public RawImage ObjectGImage;
    public Texture2D Texture_KB_G;
    public Texture2D Texture_KB_G_Pressed;
    public RawImage ObjectBImage;
    public Texture2D Texture_KB_B;
    public Texture2D Texture_KB_B_Pressed;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // A key
        if (Input.GetKeyDown(KeyCode.A))
        {
            ObjectAImage.texture=Texture_KB_A_Pressed;
        }
        if (Input.GetKeyUp(KeyCode.A))
        {
            ObjectAImage.texture= Texture_KB_A;
        }
        // S key
        if (Input.GetKeyDown(KeyCode.S))  
        {
        ObjectSImage.texture = Texture_KB_S_Pressed;
        }
        if (Input.GetKeyUp(KeyCode.S))
        {
        ObjectSImage.texture = Texture_KB_S;
        }

        // W key
        if (Input.GetKeyDown(KeyCode.W))
        {
        ObjectWImage.texture = Texture_KB_W_Pressed;  
        }
        if (Input.GetKeyUp(KeyCode.W))
        {
        ObjectWImage.texture = Texture_KB_W;
        }

        // D key
        if (Input.GetKeyDown(KeyCode.D))
        {
        ObjectDImage.texture = Texture_KB_D_Pressed;
        } 
        if (Input.GetKeyUp(KeyCode.D))
        {
        ObjectDImage.texture = Texture_KB_D;
        }
        // Space key
        if(Input.GetKeyDown(KeyCode.Space)){
        ObjectSpaceImage.texture = Texture_KB_Space_Pressed;
        }
        if(Input.GetKeyUp(KeyCode.Space)){
        ObjectSpaceImage.texture = Texture_KB_Space;
        }
        // Enter key
        if(Input.GetKeyDown(KeyCode.Return)){
        ObjectEnterImage.texture = Texture_KB_Enter_Pressed;   
        }
        if(Input.GetKeyUp(KeyCode.Return)){
        ObjectEnterImage.texture = Texture_KB_Enter;
        }
        // C key
        if(Input.GetKeyDown(KeyCode.C)){
        ObjectCImage.texture = Texture_KB_C_Pressed;
        }
        if(Input.GetKeyUp(KeyCode.C)){
        ObjectCImage.texture = Texture_KB_C;   
        }
        // R key
        if(Input.GetKeyDown(KeyCode.R)){
        ObjectRImage.texture = Texture_KB_R_Pressed;
        }
        if(Input.GetKeyUp(KeyCode.R)){
        ObjectRImage.texture = Texture_KB_R;
        }
        // G key
        if(Input.GetKeyDown(KeyCode.G)){
        ObjectGImage.texture = Texture_KB_G_Pressed;
        } 
        if(Input.GetKeyUp(KeyCode.G)){
        ObjectGImage.texture = Texture_KB_G;
        }
        // B key
        if(Input.GetKeyDown(KeyCode.B)){
        ObjectBImage.texture = Texture_KB_B_Pressed;
        }
        if(Input.GetKeyUp(KeyCode.B)){
        ObjectBImage.texture = Texture_KB_B;
        }
        
    }
}
