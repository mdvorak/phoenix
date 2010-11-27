using System;
using System.Collections.Generic;
using System.Text;
using Phoenix.WorldData;

namespace Phoenix
{
    public partial class UO
    {
        /// <summary>
        /// Waits the target object.
        /// </summary>
        /// <param name="serial">The serial.</param>
        /// <returns></returns>
        [Command]
        public static IRequestResult WaitTargetObject(Serial serial)
        {
            if (serial.IsValid) {
                RealObject obj = World.GetRealObject(serial);
                return UIManager.WaitForTarget(serial, obj.X, obj.Y, obj.Z, 0);
            }
            else {
                ScriptErrorException.Throw("Invalid serial specified to WaitTargetObject.");
                return UIManager.FailedResult;
            }
        }

        /// <summary>
        /// Waits the target self.
        /// </summary>
        /// <returns></returns>
        [Command]
        public static IRequestResult WaitTargetSelf()
        {
            return UIManager.WaitForTarget(World.PlayerSerial, World.RealPlayer.X, World.RealPlayer.Y, World.RealPlayer.Z, 0);
        }

        /// <summary>
        /// Waits the target last.
        /// </summary>
        /// <returns></returns>
        [Command]
        public static IRequestResult WaitTargetLast()
        {
            return WaitTargetObject(Aliases.LastTarget);
        }

        /// <summary>
        /// Waits the target cancel.
        /// </summary>
        /// <returns></returns>
        [Command]
        public static IRequestResult WaitTargetCancel()
        {
            return UIManager.WaitForTarget(0, 0xFFFF, 0xFFFF, 0, 0);
        }

        /// <summary>
        /// Waits the menu.
        /// </summary>
        /// <param name="menus">The menus.</param>
        /// <returns></returns>
        [Command]
        public static IRequestResult WaitMenu(params string[] menus)
        {
            if (menus.Length > 0) {
                MenuSelection[] menuList = new MenuSelection[menus.Length / 2 + menus.Length % 2];

                for (int i = 0; i < menuList.Length; i++) {
                    menuList[i].Name = menus[i * 2];

                    if (i * 2 + 1 < menus.Length)
                        menuList[i].Option = menus[i * 2 + 1];
                }

                return UIManager.WaitForMenu(menuList);
            }
            else {
                ScriptErrorException.Throw("Invalid number of parameters.");
                return UIManager.FailedResult;
            }
        }

        /// <summary>
        /// Waits the type of the target.
        /// </summary>
        /// <param name="graphic">The graphic.</param>
        /// <returns></returns>
        [Command]
        public static IRequestResult WaitTargetType(Graphic graphic)
        {
            return WaitTargetType(graphic, UOColor.Invariant);
        }

        /// <summary>
        /// Waits the type of the target.
        /// </summary>
        /// <param name="graphic">The graphic.</param>
        /// <param name="color">The color.</param>
        /// <returns></returns>
        [Command]
        public static IRequestResult WaitTargetType(Graphic graphic, UOColor color)
        {
            UOItem item = World.Player.Layers.FindType(graphic, color);

            if (item.Serial.IsValid)
                return WaitTargetObject(item);
            else {
                ScriptErrorException.Throw("Type not found.");
                return UIManager.FailedResult;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="staticGraphic">Graphic of targetted static. Use 0 to target ground.</param>
        [Command]
        public static IRequestResult WaitTargetTile(ushort x, ushort y, sbyte z, ushort staticGraphic)
        {
            return UIManager.WaitForTarget(0, x, y, z, staticGraphic);
        }

        /// <summary>
        /// Targets tile at player-relative coordinates.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="staticGraphic"></param>
        [Command]
        public static IRequestResult WaitTargetTileRel(int relX, int relY, sbyte relZ, ushort staticGraphic)
        {
            return UIManager.WaitForTarget(new RelativeTarget(relX, relY, relZ, staticGraphic));
        }
    }
}
