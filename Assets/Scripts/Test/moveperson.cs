using UnityEngine;
using System.Collections;
using System.Linq;
public class moveperson : MonoBehaviour
{
	private int maleState;//角色状态
	private int oldMaleState=0;//前一次角色的状态
	private int MALE_UP = 0;//角色状态向前
	private int MALE_RIGHT =1;//角色状态向右
	private int MALE_DOWN = 2;//角色状态向后
	private int MALE_LEFT = 3;//角色状态向左

	public float speed=8;
		
		void Start()
		{
	}
	void Update()
	{
		if (Input.GetKey("w"))
		{
			setMalemaleState(MALE_UP);
		}
		else if (Input.GetKey("s"))
		{
			setMalemaleState(MALE_DOWN);
		}
		
		if (Input.GetKey("a"))
		{
			setMalemaleState(MALE_LEFT);
		}
		else if (Input.GetKey("d"))
		{
			setMalemaleState(MALE_RIGHT);
		}
		
	}
	

	void setMalemaleState(int currMaleState)
	{
		Vector3 transformValue = new Vector3();//定义平移向量
		int rotateValue = (currMaleState - maleState) * 90;
		transform.FindChild("Male").GetComponent<Animation>().Play("walk");//播放角色行走动画
		switch (currMaleState)
		{
		case 0://角色状态向前时，角色不断向前缓慢移动
			transformValue = Vector3.forward * Time.deltaTime * speed;
			break;
		case 1://角色状态向右时，角色不断向右缓慢移动
			transformValue = Vector3.right * Time.deltaTime * speed;
			break;
		case 2://角色状态向后时，角色不断向后缓慢移动
			transformValue = Vector3.back * Time.deltaTime * speed;
			break;
		case 3://角色状态向左时，角色不断向左缓慢移动
			transformValue = Vector3.left * Time.deltaTime * speed;
			break;
		}
		transform.FindChild("Male").Rotate(Vector3.up, rotateValue);//旋转角色
		transform.Translate(transformValue, Space.World);//平移角色
		oldMaleState = maleState;//赋值，方便下一次计算
		maleState = currMaleState;//赋值，方便下一次计算
	}
	
	
}