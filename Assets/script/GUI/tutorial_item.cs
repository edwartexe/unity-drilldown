using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tutorial_item {

    public Sprite Image { get; set; }
    public string Text { get; set; }

    public tutorial_item(Sprite image, string text) {
        this.Image = image;
        this.Text = text;
    }
}
