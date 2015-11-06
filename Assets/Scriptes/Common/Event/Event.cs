using System;

class Event
{
    // Fields
    private object context;
    private string name;
    private object[] data;
    

    // Methods
    public Event(object context, string name, object[] data)
    {
        this.context = context;
        this.name = name;
        this.data = data;
    }

    // Properties
    public object Context
    {
        get
        {
            return this.context;
        }
    }


    public object[] Data
    {
        get
        {
            return this.data;
        }
    }


    public string Name
    {
        get
        {
            return this.name;
        }
    }
}


