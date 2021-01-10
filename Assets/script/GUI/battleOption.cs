using System.Collections;
using System.Collections.Generic;
using System;

public class battleOption {
    public int code;
    public string name;
    public int cost;
    public battleOption(int _code, string _name) {
        code = _code;
        name = _name;
        cost = 0;
    }

    public battleOption(int _code, string _name,int _cost) {
        code = _code;
        name = _name;
        cost = _cost;
    }
}