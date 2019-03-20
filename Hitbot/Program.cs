using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TcpserverExDll;
using ControlBeanExDll;
using System.Threading;

namespace Hitbot
{
    class Program
    {
        public static int card_number = 10;

        public static int move_code = 0;

        public static bool _RobotReady = true;

        static void Main(string[] args)
        {
            OpenTCPServer();

            ControlBeanEx Robot = TcpserverEx.get_robot(card_number);

            //Robot.set_allow_offset_at_target_position(10, 10, 10, 10);

            UnlockRobot(Robot);
            InitialRobot(Robot);

            //SetDragTeach(Robot, true);

            int _case = 0;

            while (true)
            {
                try
                {
                    switch (_case)
                    {
                        case 0:
                            {
                                if (_RobotReady)
                                {
                                    move_code = RobotSetPositionMove(Robot, 100, -50, -30, 0, 50);
                                    _RobotReady = false;
                                }
                                if (RobotMoveOver(Robot))
                                {
                                    _RobotReady = true;
                                    _case = 1;
                                }
                            }
                            break;
                        case 1:
                            {
                                if (_RobotReady)
                                {
                                    move_code = RobotAngleMove(Robot, 20, 20, -50, 0, 50);
                                    _RobotReady = false;
                                }
                                if (RobotMoveOver(Robot))
                                {
                                    _RobotReady = true;
                                    _case = 2;
                                }
                            }
                            break;
                        case 2:
                            {
                                if (_RobotReady)
                                {
                                    move_code = RobotChangeAttitide(Robot, 50);
                                    _RobotReady = false;
                                }
                                if (RobotMoveOver(Robot))
                                {
                                    _RobotReady = true;
                                    _case = 0;
                                }
                            }
                            break;
                    }
                }
                catch
                {
                    // ignored
                }
            }

            //CloseTCPServer();
        }

        // 开启Server服务
        static void OpenTCPServer()
        {
            TcpserverEx.net_port_initial();
            Thread.Sleep(500);
        }

        // 关闭Server服务
        static void CloseTCPServer()
        {
            TcpserverEx.close_tcpserver();
            KillProcessHelper.KillProcess("server");
            KillProcessHelper.RefreshTrayIcon();
            KillProcessHelper.RefreshNotifyIcon();
        }

        // 解锁机械手
        static int UnlockRobot(ControlBeanEx robot)
        {
            int unlock_code = robot.unlock_position();
            return unlock_code;
        }

        // 初始化机械手
        static int InitialRobot(ControlBeanEx robot)
        {
            int initial_code = robot.initial(1, 210);
            return initial_code;
        }

        // position运动模式
        static int RobotSetPositionMove(ControlBeanEx robot, float goal_x, float goal_y, float goal_z, float rotation, float speed)
        {
            int move_code = robot.set_position_move(goal_x, goal_y, goal_z, rotation, speed, 1, 1, 1);
            return move_code;
        }

        // angle运动模式
        static int RobotAngleMove(ControlBeanEx robot, float angle1, float angle2, float z, float rotation, float speed)
        {
            int move_code = robot.set_angle_move(angle1, angle2, z, rotation, speed);
            return move_code;
        }

        // 机械手连接检测
        static int RobotConnect(int card_number)
        {
            int connect_code = TcpserverEx.card_number_connect(card_number);
            return connect_code;
        }

        // 设置拖动示教模式
        static bool SetDragTeach(ControlBeanEx robot, bool state)
        {
            bool ret_code = robot.set_drag_teach(state);
            return ret_code;
        }

        // 机械手变换手势
        static int RobotChangeAttitide(ControlBeanEx robot, float speed)
        {
            int change_attitude_code = robot.change_attitude(speed);
            return change_attitude_code;
        }

        // 机械手各轴就绪
        static bool RobotJointReady(ControlBeanEx robot)
        {
            int sum_joint = 0;
            for (int i = 1; i <= 4; i++)
            {
                sum_joint += robot.get_joint_state(i);
            }
            return sum_joint == 4 ? true : false;
        }

        // 机械手运动指令完成检测
        static bool RobotMoveOver(ControlBeanEx robot)
        {
            robot.get_scara_param();

            // 机械手运动指令完成判定条件
            if ((!robot.move_flag) && robot.is_robot_goto_target() && (RobotJointReady(robot)) && (move_code == 1))
            {
                move_code = 0;
                return true;
            }
            else
            {
                return false;
            }
        }

        // 机械手位置数据
        public static void GetRobotData(ControlBeanEx robot, out float x, out float y, out float z, out float r, out float angle1, out float angle2)
        {
            robot.get_scara_param();
            x = robot.x;
            y = robot.y;
            z = robot.z;
            r = robot.rotation;
            angle1 = robot.angle1;
            angle2 = robot.angle2;
        }
    }
}
