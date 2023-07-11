using System.Collections;
using System.Collections.Generic;


public class GameDataValue 
{
    public GameDataValue(dynamic defaultVal)
    {
        m_default = defaultVal;
        m_current = defaultVal;
    }
    public dynamic m_default;
    public dynamic m_current;
    public bool m_set;
}

