using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
//using Random = UnityEngine.Random;

public class Spot_TY_Controller : Agent
{
    public float point;
    public GameObject[] Joint;
    public GameObject[] Link; //12개 관절
    //public GameObject body;
    public GameObject[] collision_part;
    public float[] AngleLimit_start; //최소 각도를 저장
    public float[] AngleLimit_end; //최대 각도를 저장
    public float[] JointAngle; //i번째 관절 각도
    public int JointIdx; //지정 관절 번호
    public int speed; //관절 제어 속도

    Vector3 start_pos;
    Quaternion start_rot;

    public Quaternion sensor_rot;
    public Vector3 sensor_vel;

    public override void Initialize()
    {
        //관절변수 초기화
        start_pos = transform.position;
        start_rot = transform.rotation;
    }

    void Update()
    {
        Manual_Input(); //수동으로 조종
        JointUpdate(); //관절 상태 업데이트
        if (Input.GetKeyUp(KeyCode.R)) //수동 초기화
        {
            Spot_Initialize();
        }

        GiveReward(); //보상 주기

        //센서값
        sensor_rot = transform.rotation;
        Rigidbody rigid;
        rigid = gameObject.GetComponent<Rigidbody>();
        sensor_vel = rigid.velocity;
    }

    void GiveReward()
    {
        //z값 커지면 전진 ... 보상이다.
        Rigidbody rigid;
        rigid = this.GetComponent<Rigidbody>();
        float v_size = rigid.velocity.x * rigid.velocity.x + rigid.velocity.y * rigid.velocity.y +
            rigid.velocity.z * rigid.velocity.z;
        if (v_size > 0.01f)
        {
            AddReward(v_size * 1f);
            point += v_size * 1f;
        }
        //넘어지면 ... 처벌이다.
        //Debug.Log("z:" + transform.eulerAngles.z + " x:" + transform.eulerAngles.x);
        if (transform.eulerAngles.z > 30 && transform.eulerAngles.z < 330 ||
            transform.eulerAngles.x > 50 && transform.eulerAngles.x < 310)
        {
            AddReward(-10f);
            point -= 10f;
            EndEpisode();
        }
        //Debug.Log(transform.position.y);
        if (transform.position.y < 0.05)
        {
            AddReward(-10f);
            point -= 10f;
            EndEpisode();
        }
        /*
        Spot_Collision code_collision_0;
        Spot_Collision code_collision_1;
        Spot_Collision code_collision_2;
        Spot_Collision code_collision_3;
        Spot_Collision code_collision_4;
        code_collision_0 = collision_part[0].GetComponent<Spot_Collision>();
        code_collision_1 = collision_part[1].GetComponent<Spot_Collision>();
        code_collision_2 = collision_part[2].GetComponent<Spot_Collision>();
        code_collision_3 = collision_part[3].GetComponent<Spot_Collision>();
        code_collision_4 = collision_part[4].GetComponent<Spot_Collision>();
        if (code_collision_0.isCollision == true || code_collision_1.isCollision == true ||
            code_collision_2.isCollision == true || code_collision_3.isCollision == true ||
            code_collision_4.isCollision == true)
        {
            code_collision_0.isCollision = false;
            code_collision_1.isCollision = false;
            code_collision_2.isCollision = false;
            code_collision_3.isCollision = false;
            code_collision_4.isCollision = false;
            AddReward(-10f);
            EndEpisode();
        }
        */

    }

    /* OnCollisionEnter()
         * rear_right_upper_leg_collision_0
         * rear_left_upper_leg_collision_0
         * front_left_upper_leg_collision_0
         * front_right_upper_leg_collision_0
         * body_collision_0
    */
    /*
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.name == "rear_right_upper_leg_collision_0" || collision.gameObject.name == "rear_left_upper_leg_collision_0" ||
            collision.gameObject.name == "front_left_upper_leg_collision_0" || collision.gameObject.name == "front_right_upper_leg_collision_0" ||
            collision.gameObject.name == "body_collision_0")
        {
            AddReward(-10f);
            EndEpisode();
        }
    }
    */

    public override void CollectObservations(VectorSensor sensor)
    {
        //각 관절 각도
        for(int i = 0; i<Joint.Length; i++)
        {
            sensor.AddObservation(JointAngle[i]);
        }

        //현재몸체 각도
        sensor.AddObservation(transform.rotation);

        //현재 몸체 속도
        Rigidbody rigid;
        rigid = gameObject.GetComponent<Rigidbody>();
        sensor.AddObservation(rigid.velocity);
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        var da = actionBuffers.DiscreteActions;
        for (int i = 0; i < Joint.Length; i++)
        {
            
            if (JointAngle[i]+ (da[i] - 1) * speed < AngleLimit_start[i] ||
                JointAngle[i] + (da[i] - 1) * speed > AngleLimit_end[i]) continue;
            //Debug.Log("da[" + i + "]:" + da[i]);
            
            JointAngle[i] += (da[i]-1)*speed;
        }
    }

    public override void OnEpisodeBegin()
    {
        Spot_Initialize();
    }

    //사용자 입력으로 관절 제어
    void Manual_Input()
    {
        //관절 번호
        if (Input.GetKeyUp(KeyCode.Q))
        {
            Link[JointIdx].GetComponent<MeshRenderer>().material.color = Color.white; //원래색으로
            JointIdx++;
        }
        else if (Input.GetKeyUp(KeyCode.E))
        {
            Link[JointIdx].GetComponent<MeshRenderer>().material.color = Color.white; //원래색으로
            JointIdx--;
        }
        if (JointIdx < 0) JointIdx = 0;
        else if (JointIdx >= Joint.Length) JointIdx = Joint.Length-1;
        
        Link[JointIdx].GetComponent<MeshRenderer>().material.color = Color.red; //해당 관절 색 빨간색

        //속도
        if (Input.GetKeyUp(KeyCode.Z)) speed++;
        else if (Input.GetKeyUp(KeyCode.C)) speed--;
        if (speed > 10) speed = 10;
        else if (speed < 1) speed = 1;

        //방향 
        if (Input.GetKeyUp(KeyCode.D) && AngleLimit_end[JointIdx] >= JointAngle[JointIdx] + speed)
        {
            JointAngle[JointIdx] += speed;
        }
        else if (Input.GetKeyUp(KeyCode.A) && AngleLimit_start[JointIdx] <= JointAngle[JointIdx]- speed)
        {
            JointAngle[JointIdx] -= speed;
        }
    }

    //관절 상태 업데이트
    void JointUpdate()
    {
        for(int i = 0; i<Joint.Length; i++)
        {
            //Joint[i].transform.Rotate(0, 20, 0, Space.Self);
            Vector3 angle = Joint[i].transform.localEulerAngles;
            angle.y = JointAngle[i];
            Joint[i].transform.localEulerAngles = angle;
        }
    }
    
    //관절 초기화
    void Spot_Initialize()
    {
        point = 0;
        JointIdx = 0;
        speed = 1;
        for (int i = 0; i < Joint.Length; i++)
        {
            JointAngle[i] = 0;
            Link[i].GetComponent<MeshRenderer>().material.color = Color.white;
        }
        transform.position = start_pos;
        transform.rotation = start_rot;
        Rigidbody rigid;
        rigid = gameObject.GetComponent<Rigidbody>();
        rigid.velocity = new Vector3(0,0,0);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {

    }
}
