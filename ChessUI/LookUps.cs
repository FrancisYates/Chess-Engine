﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessUI
{
    public static class LookUps
    {
        #region pawnAttackOffset
        public readonly static int[,][] pawnAttackOffset = { { new int[] { -7 }, new int[] { -9, -7 }, new int[] { -9, -7 }, new int[] { -9, -7 }, new int[] { -9, -7 }, new int[] { -9, -7 }, new int[] { -9, -7 }, new int[] { -9 }, new int[] { -7 }, new int[] { -9, -7 }, new int[] { -9, -7 }, new int[] { -9, -7 }, new int[] { -9, -7 }, new int[] { -9, -7 }, new int[] { -9, -7 }, new int[] { -9 }, new int[] { -7 }, new int[] { -9, -7 }, new int[] { -9, -7 }, new int[] { -9, -7 }, new int[] { -9, -7 }, new int[] { -9, -7 }, new int[] { -9, -7 }, new int[] { -9 }, new int[] { -7 }, new int[] { -9, -7 }, new int[] { -9, -7 }, new int[] { -9, -7 }, new int[] { -9, -7 }, new int[] { -9, -7 }, new int[] { -9, -7 }, new int[] { -9 }, new int[] { -7 }, new int[] { -9, -7 }, new int[] { -9, -7 }, new int[] { -9, -7 }, new int[] { -9, -7 }, new int[] { -9, -7 }, new int[] { -9, -7 }, new int[] { -9 }, new int[] { -7 }, new int[] { -9, -7 }, new int[] { -9, -7 }, new int[] { -9, -7 }, new int[] { -9, -7 }, new int[] { -9, -7 }, new int[] { -9, -7 }, new int[] { -9 }, new int[] { -7 }, new int[] { -9, -7 }, new int[] { -9, -7 }, new int[] { -9, -7 }, new int[] { -9, -7 }, new int[] { -9, -7 }, new int[] { -9, -7 }, new int[] { -9 }, new int[] { -7 }, new int[] { -9, -7 }, new int[] { -9, -7 }, new int[] { -9, -7 }, new int[] { -9, -7 }, new int[] { -9, -7 }, new int[] { -9, -7 } }, { new int[] { 9 }, new int[] { 7, 9 }, new int[] { 7, 9 }, new int[] { 7, 9 }, new int[] { 7, 9 }, new int[] { 7, 9 }, new int[] { 7, 9 }, new int[] { 7 }, new int[] { 9 }, new int[] { 7, 9 }, new int[] { 7, 9 }, new int[] { 7, 9 }, new int[] { 7, 9 }, new int[] { 7, 9 }, new int[] { 7, 9 }, new int[] { 7 }, new int[] { 9 }, new int[] { 7, 9 }, new int[] { 7, 9 }, new int[] { 7, 9 }, new int[] { 7, 9 }, new int[] { 7, 9 }, new int[] { 7, 9 }, new int[] { 7 }, new int[] { 9 }, new int[] { 7, 9 }, new int[] { 7, 9 }, new int[] { 7, 9 }, new int[] { 7, 9 }, new int[] { 7, 9 }, new int[] { 7, 9 }, new int[] { 7 }, new int[] { 9 }, new int[] { 7, 9 }, new int[] { 7, 9 }, new int[] { 7, 9 }, new int[] { 7, 9 }, new int[] { 7, 9 }, new int[] { 7, 9 }, new int[] { 7 }, new int[] { 9 }, new int[] { 7, 9 }, new int[] { 7, 9 }, new int[] { 7, 9 }, new int[] { 7, 9 }, new int[] { 7, 9 }, new int[] { 7, 9 }, new int[] { 7 }, new int[] { 9 }, new int[] { 7, 9 }, new int[] { 7, 9 }, new int[] { 7, 9 }, new int[] { 7, 9 }, new int[] { 7, 9 }, new int[] { 7, 9 }, new int[] { 7 }, new int[] { 9 }, new int[] { 7, 9 }, new int[] { 7, 9 }, new int[] { 7, 9 }, new int[] { 7, 9 }, new int[] { 7, 9 }, new int[] { 7, 9 } } };
        #endregion pawnAttackOffset

        #region knightOffset
        public readonly static int[][] knightOffset = {new int[] {17, 10}, new int[] {15, 17, 10}, new int[] {15, 17, 10, 6}, new int[] {15, 17, 10, 6}, new int[] {15, 17, 10, 6}, new int[] {15, 17, 10, 6}, new int[] {15, 17, 6}, new int[] {15, 6}, new int[] {17, 10, -6}, new int[] {15, 17, 10, -6}, new int[] {15, 17, 10, -6, -10, 6}, new int[] {15, 17, 10, -6, -10, 6}, new int[] {15, 17, 10, -6, -10, 6}, new int[] {15, 17, 10, -6, -10, 6}, new int[] {15, 17, -10, 6}, new int[] {15, -10, 6}, new int[] {17, 10, -6, -15}, new int[] {15, 17, 10, -6, -15, -17}, new int[] {15, 17, 10, -6, -15, -17, -10, 6}, new int[] {15, 17, 10, -6, -15, -17, -10, 6}, new int[] {15, 17, 10, -6, -15, -17, -10, 6}, new int[] {15, 17, 10, -6, -15, -17, -10, 6}, new int[] {15, 17, -15, -17, -10, 6}, new int[] {15, -17, -10, 6}, new int[] {17, 10, -6, -15}, new int[] {15, 17, 10, -6, -15, -17}, new int[] {15, 17, 10, -6, -15, -17, -10, 6}, new int[] {15, 17, 10, -6, -15, -17, -10, 6}, new int[] {15, 17, 10, -6, -15, -17, -10, 6}, new int[] {15, 17, 10, -6, -15, -17, -10, 6}, new int[] {15, 17, -15, -17, -10, 6}, new int[] {15, -17, -10, 6}, new int[] {17, 10, -6, -15}, new int[] {15, 17, 10, -6, -15, -17}, new int[] {15, 17, 10, -6, -15, -17, -10, 6}, new int[] {15, 17, 10, -6, -15, -17, -10, 6}, new int[] {15, 17, 10, -6, -15, -17, -10, 6}, new int[] {15, 17, 10, -6, -15, -17, -10, 6}, new int[] {15, 17, -15, -17, -10, 6}, new int[] {15, -17, -10, 6}, new int[] {17, 10, -6, -15}, new int[] {15, 17, 10, -6, -15, -17}, new int[] {15, 17, 10, -6, -15, -17, -10, 6}, new int[] {15, 17, 10, -6, -15, -17, -10, 6}, new int[] {15, 17, 10, -6, -15, -17, -10, 6}, new int[] {15, 17, 10, -6, -15, -17, -10, 6}, new int[] {15, 17, -15, -17, -10, 6}, new int[] {15, -17, -10, 6}, new int[] {10, -6, -15}, new int[] {10, -6, -15, -17}, new int[] {10, -6, -15, -17, -10, 6}, new int[] {10, -6, -15, -17, -10, 6}, new int[] {10, -6, -15, -17, -10, 6}, new int[] {10, -6, -15, -17, -10, 6}, new int[] {-15, -17, -10, 6}, new int[] {-17, -10, 6}, new int[] {-6, -15}, new int[] {-6, -15, -17}, new int[] {-6, -15, -17, -10}, new int[] {-6, -15, -17, -10}, new int[] {-6, -15, -17, -10}, new int[] {-6, -15, -17, -10}, new int[] {-15, -17, -10}, new int[] {-17, -10}};
        #endregion knightOffset

        #region kingOffset
        public readonly static int[][] kingOffset = {new int[] {8, 1, 9}, new int[] {8, -1, 1, 7, 9}, new int[] { 8, -1, 1, 7, 9 }, new int[] { 8, -1, 1, 7, 9 }, new int[] { 8, -1, 1, 7, 9 }, new int[] { 8, -1, 1, 7, 9 }, new int[] { 8, -1, 1, 7, 9 }, new int[] { 8, -1, 7 }, new int[] { 8, -8, 1, 9, -7 }, new int[] { 8, -8, -1, 1, 7, 9, -7, -9 }, new int[] { 8, -8, -1, 1, 7, 9, -7, -9 }, new int[] { 8, -8, -1, 1, 7, 9, -7, -9 }, new int[] { 8, -8, -1, 1, 7, 9, -7, -9 }, new int[] { 8, -8, -1, 1, 7, 9, -7, -9 }, new int[] { 8, -8, -1, 1, 7, 9, -7, -9 }, new int[] { 8, -8, -1, 7, -9 }, new int[] { 8, -8, 1, 9, -7 }, new int[] { 8, -8, -1, 1, 7, 9, -7, -9 }, new int[] { 8, -8, -1, 1, 7, 9, -7, -9 }, new int[] { 8, -8, -1, 1, 7, 9, -7, -9 }, new int[] { 8, -8, -1, 1, 7, 9, -7, -9 }, new int[] { 8, -8, -1, 1, 7, 9, -7, -9 }, new int[] { 8, -8, -1, 1, 7, 9, -7, -9 }, new int[] { 8, -8, -1, 7, -9 }, new int[] { 8, -8, 1, 9, -7 }, new int[] {8, -8, -1, 1, 7, 9, -7, -9}, new int[] { 8, -8, -1, 1, 7, 9, -7, -9 }, new int[] { 8, -8, -1, 1, 7, 9, -7, -9 }, new int[] { 8, -8, -1, 1, 7, 9, -7, -9 }, new int[] { 8, -8, -1, 1, 7, 9, -7, -9 }, new int[] { 8, -8, -1, 1, 7, 9, -7, -9 }, new int[] { 8, -8, -1, 7, -9 }, new int[] { 8, -8, 1, 9, -7 }, new int[] { 8, -8, -1, 1, 7, 9, -7, -9 }, new int[] { 8, -8, -1, 1, 7, 9, -7, -9 }, new int[] { 8, -8, -1, 1, 7, 9, -7, -9 }, new int[] { 8, -8, -1, 1, 7, 9, -7, -9 }, new int[] { 8, -8, -1, 1, 7, 9, -7, -9 }, new int[] { 8, -8, -1, 1, 7, 9, -7, -9 }, new int[] { 8, -8, -1, 7, -9 }, new int[] { 8, -8, 1, 9, -7 }, new int[] { 8, -8, -1, 1, 7, 9, -7, -9 }, new int[] { 8, -8, -1, 1, 7, 9, -7, -9 }, new int[] { 8, -8, -1, 1, 7, 9, -7, -9 }, new int[] { 8, -8, -1, 1, 7, 9, -7, -9 }, new int[] { 8, -8, -1, 1, 7, 9, -7, -9 }, new int[] { 8, -8, -1, 1, 7, 9, -7, -9 }, new int[] { 8, -8, -1, 7, -9 }, new int[] { 8, -8, 1, 9, -7 }, new int[] {8, -8, -1, 1, 7, 9, -7, -9}, new int[] { 8, -8, -1, 1, 7, 9, -7, -9 }, new int[] { 8, -8, -1, 1, 7, 9, -7, -9 }, new int[] { 8, -8, -1, 1, 7, 9, -7, -9 }, new int[] { 8, -8, -1, 1, 7, 9, -7, -9 }, new int[] { 8, -8, -1, 1, 7, 9, -7, -9 }, new int[] { 8, -8, -1, 7, -9 }, new int[] { -8, 1, -7 }, new int[] { -8, -1, 1, -7, -9 }, new int[] { -8, -1, 1, -7, -9 }, new int[] { -8, -1, 1, -7, -9 }, new int[] { -8, -1, 1, -7, -9 }, new int[] { -8, -1, 1, -7, -9 }, new int[] { -8, -1, 1, -7, -9 }, new int[] { -8, -1, -9 }};
            #endregion kingOffset

        #region dirextionIdx
        public static readonly int[,] directionIndex = new int[,] {{-1, 3, 3, 3, 3, 3, 3, 3, 0, 5, -1, -1, -1, -1, -1, -1, 0, -1, 5, -1, -1, -1, -1, -1, 0, -1, -1, 5, -1, -1, -1, -1, 0, -1, -1, -1, 5, -1, -1, -1, 0, -1, -1, -1, -1, 5, -1, -1, 0, -1, -1, -1, -1, -1, 5, -1, 0, -1, -1, -1, -1, -1, -1, 5}, {2, -1, 3, 3, 3, 3, 3, 3, 4, 0, 5, -1, -1, -1, -1, -1, -1, 0, -1, 5, -1, -1, -1, -1, -1, 0, -1, -1, 5, -1, -1, -1, -1, 0, -1, -1, -1, 5, -1, -1, -1, 0, -1, -1, -1, -1, 5, -1, -1, 0, -1, -1, -1, -1, -1, 5, -1, 0, -1, -1, -1, -1, -1, -1}, {2, 2, -1, 3, 3, 3, 3, 3, -1, 4, 0, 5, -1, -1, -1, -1, 4, -1, 0, -1, 5, -1, -1, -1, -1, -1, 0, -1, -1, 5, -1, -1, -1, -1, 0, -1, -1, -1, 5, -1, -1, -1, 0, -1, -1, -1, -1, 5, -1, -1, 0, -1, -1, -1, -1, -1, -1, -1, 0, -1, -1, -1, -1, -1}, {2, 2, 2, -1, 3, 3, 3, 3, -1, -1, 4, 0, 5, -1, -1, -1, -1, 4, -1, 0, -1, 5, -1, -1, 4, -1, -1, 0, -1, -1, 5, -1, -1, -1, -1, 0, -1, -1, -1, 5, -1, -1, -1, 0, -1, -1, -1, -1, -1, -1, -1, 0, -1, -1, -1, -1, -1, -1, -1, 0, -1, -1, -1, -1}, {2, 2, 2, 2, -1, 3, 3, 3, -1, -1, -1, 4, 0, 5, -1, -1, -1, -1, 4, -1, 0, -1, 5, -1, -1, 4, -1, -1, 0, -1, -1, 5, 4, -1, -1, -1, 0, -1, -1, -1, -1, -1, -1, -1, 0, -1, -1, -1, -1, -1, -1, -1, 0, -1, -1, -1, -1, -1, -1, -1, 0, -1, -1, -1}, {2, 2, 2, 2, 2, -1, 3, 3, -1, -1, -1, -1, 4, 0, 5, -1, -1, -1, -1, 4, -1, 0, -1, 5, -1, -1, 4, -1, -1, 0, -1, -1, -1, 4, -1, -1, -1, 0, -1, -1, 4, -1, -1, -1, -1, 0, -1, -1, -1, -1, -1, -1, -1, 0, -1, -1, -1, -1, -1, -1, -1, 0, -1, -1}, {2, 2, 2, 2, 2, 2, -1, 3, -1, -1, -1, -1, -1, 4, 0, 5, -1, -1, -1, -1, 4, -1, 0, -1, -1, -1, -1, 4, -1, -1, 0, -1, -1, -1, 4, -1, -1, -1, 0, -1, -1, 4, -1, -1, -1, -1, 0, -1, 4, -1, -1, -1, -1, -1, 0, -1, -1, -1, -1, -1, -1, -1, 0, -1}, {2, 2, 2, 2, 2, 2, 2, -1, -1, -1, -1, -1, -1, -1, 4, 0, -1, -1, -1, -1, -1, 4, -1, 0, -1, -1, -1, -1, 4, -1, -1, 0, -1, -1, -1, 4, -1, -1, -1, 0, -1, -1, 4, -1, -1, -1, -1, 0, -1, 4, -1, -1, -1, -1, -1, 0, 4, -1, -1, -1, -1, -1, -1, 0}, {1, 7, -1, -1, -1, -1, -1, -1, -1, 3, 3, 3, 3, 3, 3, 3, 0, 5, -1, -1, -1, -1, -1, -1, 0, -1, 5, -1, -1, -1, -1, -1, 0, -1, -1, 5, -1, -1, -1, -1, 0, -1, -1, -1, 5, -1, -1, -1, 0, -1, -1, -1, -1, 5, -1, -1, 0, -1, -1, -1, -1, -1, 5, -1}, {6, 1, 7, -1, -1, -1, -1, -1, 2, -1, 3, 3, 3, 3, 3, 3, 4, 0, 5, -1, -1, -1, -1, -1, -1, 0, -1, 5, -1, -1, -1, -1, -1, 0, -1, -1, 5, -1, -1, -1, -1, 0, -1, -1, -1, 5, -1, -1, -1, 0, -1, -1, -1, -1, 5, -1, -1, 0, -1, -1, -1, -1, -1, 5}, {-1, 6, 1, 7, -1, -1, -1, -1, 2, 2, -1, 3, 3, 3, 3, 3, -1, 4, 0, 5, -1, -1, -1, -1, 4, -1, 0, -1, 5, -1, -1, -1, -1, -1, 0, -1, -1, 5, -1, -1, -1, -1, 0, -1, -1, -1, 5, -1, -1, -1, 0, -1, -1, -1, -1, 5, -1, -1, 0, -1, -1, -1, -1, -1}, {-1, -1, 6, 1, 7, -1, -1, -1, 2, 2, 2, -1, 3, 3, 3, 3, -1, -1, 4, 0, 5, -1, -1, -1, -1, 4, -1, 0, -1, 5, -1, -1, 4, -1, -1, 0, -1, -1, 5, -1, -1, -1, -1, 0, -1, -1, -1, 5, -1, -1, -1, 0, -1, -1, -1, -1, -1, -1, -1, 0, -1, -1, -1, -1}, {-1, -1, -1, 6, 1, 7, -1, -1, 2, 2, 2, 2, -1, 3, 3, 3, -1, -1, -1, 4, 0, 5, -1, -1, -1, -1, 4, -1, 0, -1, 5, -1, -1, 4, -1, -1, 0, -1, -1, 5, 4, -1, -1, -1, 0, -1, -1, -1, -1, -1, -1, -1, 0, -1, -1, -1, -1, -1, -1, -1, 0, -1, -1, -1}, {-1, -1, -1, -1, 6, 1, 7, -1, 2, 2, 2, 2, 2, -1, 3, 3, -1, -1, -1, -1, 4, 0, 5, -1, -1, -1, -1, 4, -1, 0, -1, 5, -1, -1, 4, -1, -1, 0, -1, -1, -1, 4, -1, -1, -1, 0, -1, -1, 4, -1, -1, -1, -1, 0, -1, -1, -1, -1, -1, -1, -1, 0, -1, -1}, {-1, -1, -1, -1, -1, 6, 1, 7, 2, 2, 2, 2, 2, 2, -1, 3, -1, -1, -1, -1, -1, 4, 0, 5, -1, -1, -1, -1, 4, -1, 0, -1, -1, -1, -1, 4, -1, -1, 0, -1, -1, -1, 4, -1, -1, -1, 0, -1, -1, 4, -1, -1, -1, -1, 0, -1, 4, -1, -1, -1, -1, -1, 0, -1}, {-1, -1, -1, -1, -1, -1, 6, 1, 2, 2, 2, 2, 2, 2, 2, -1, -1, -1, -1, -1, -1, -1, 4, 0, -1, -1, -1, -1, -1, 4, -1, 0, -1, -1, -1, -1, 4, -1, -1, 0, -1, -1, -1, 4, -1, -1, -1, 0, -1, -1, 4, -1, -1, -1, -1, 0, -1, 4, -1, -1, -1, -1, -1, 0}, {1, -1, 7, -1, -1, -1, -1, -1, 1, 7, -1, -1, -1, -1, -1, -1, -1, 3, 3, 3, 3, 3, 3, 3, 0, 5, -1, -1, -1, -1, -1, -1, 0, -1, 5, -1, -1, -1, -1, -1, 0, -1, -1, 5, -1, -1, -1, -1, 0, -1, -1, -1, 5, -1, -1, -1, 0, -1, -1, -1, -1, 5, -1, -1}, {-1, 1, -1, 7, -1, -1, -1, -1, 6, 1, 7, -1, -1, -1, -1, -1, 2, -1, 3, 3, 3, 3, 3, 3, 4, 0, 5, -1, -1, -1, -1, -1, -1, 0, -1, 5, -1, -1, -1, -1, -1, 0, -1, -1, 5, -1, -1, -1, -1, 0, -1, -1, -1, 5, -1, -1, -1, 0, -1, -1, -1, -1, 5, -1}, {6, -1, 1, -1, 7, -1, -1, -1, -1, 6, 1, 7, -1, -1, -1, -1, 2, 2, -1, 3, 3, 3, 3, 3, -1, 4, 0, 5, -1, -1, -1, -1, 4, -1, 0, -1, 5, -1, -1, -1, -1, -1, 0, -1, -1, 5, -1, -1, -1, -1, 0, -1, -1, -1, 5, -1, -1, -1, 0, -1, -1, -1, -1, 5}, {-1, 6, -1, 1, -1, 7, -1, -1, -1, -1, 6, 1, 7, -1, -1, -1, 2, 2, 2, -1, 3, 3, 3, 3, -1, -1, 4, 0, 5, -1, -1, -1, -1, 4, -1, 0, -1, 5, -1, -1, 4, -1, -1, 0, -1, -1, 5, -1, -1, -1, -1, 0, -1, -1, -1, 5, -1, -1, -1, 0, -1, -1, -1, -1}, {-1, -1, 6, -1, 1, -1, 7, -1, -1, -1, -1, 6, 1, 7, -1, -1, 2, 2, 2, 2, -1, 3, 3, 3, -1, -1, -1, 4, 0, 5, -1, -1, -1, -1, 4, -1, 0, -1, 5, -1, -1, 4, -1, -1, 0, -1, -1, 5, 4, -1, -1, -1, 0, -1, -1, -1, -1, -1, -1, -1, 0, -1, -1, -1}, {-1, -1, -1, 6, -1, 1, -1, 7, -1, -1, -1, -1, 6, 1, 7, -1, 2, 2, 2, 2, 2, -1, 3, 3, -1, -1, -1, -1, 4, 0, 5, -1, -1, -1, -1, 4, -1, 0, -1, 5, -1, -1, 4, -1, -1, 0, -1, -1, -1, 4, -1, -1, -1, 0, -1, -1, 4, -1, -1, -1, -1, 0, -1, -1}, {-1, -1, -1, -1, 6, -1, 1, -1, -1, -1, -1, -1, -1, 6, 1, 7, 2, 2, 2, 2, 2, 2, -1, 3, -1, -1, -1, -1, -1, 4, 0, 5, -1, -1, -1, -1, 4, -1, 0, -1, -1, -1, -1, 4, -1, -1, 0, -1, -1, -1, 4, -1, -1, -1, 0, -1, -1, 4, -1, -1, -1, -1, 0, -1}, {-1, -1, -1, -1, -1, 6, -1, 1, -1, -1, -1, -1, -1, -1, 6, 1, 2, 2, 2, 2, 2, 2, 2, -1, -1, -1, -1, -1, -1, -1, 4, 0, -1, -1, -1, -1, -1, 4, -1, 0, -1, -1, -1, -1, 4, -1, -1, 0, -1, -1, -1, 4, -1, -1, -1, 0, -1, -1, 4, -1, -1, -1, -1, 0}, {1, -1, -1, 7, -1, -1, -1, -1, 1, -1, 7, -1, -1, -1, -1, -1, 1, 7, -1, -1, -1, -1, -1, -1, -1, 3, 3, 3, 3, 3, 3, 3, 0, 5, -1, -1, -1, -1, -1, -1, 0, -1, 5, -1, -1, -1, -1, -1, 0, -1, -1, 5, -1, -1, -1, -1, 0, -1, -1, -1, 5, -1, -1, -1}, {-1, 1, -1, -1, 7, -1, -1, -1, -1, 1, -1, 7, -1, -1, -1, -1, 6, 1, 7, -1, -1, -1, -1, -1, 2, -1, 3, 3, 3, 3, 3, 3, 4, 0, 5, -1, -1, -1, -1, -1, -1, 0, -1, 5, -1, -1, -1, -1, -1, 0, -1, -1, 5, -1, -1, -1, -1, 0, -1, -1, -1, 5, -1, -1}, {-1, -1, 1, -1, -1, 7, -1, -1, 6, -1, 1, -1, 7, -1, -1, -1, -1, 6, 1, 7, -1, -1, -1, -1, 2, 2, -1, 3, 3, 3, 3, 3, -1, 4, 0, 5, -1, -1, -1, -1, 4, -1, 0, -1, 5, -1, -1, -1, -1, -1, 0, -1, -1, 5, -1, -1, -1, -1, 0, -1, -1, -1, 5, -1}, {6, -1, -1, 1, -1, -1, 7, -1, -1, 6, -1, 1, -1, 7, -1, -1, -1, -1, 6, 1, 7, -1, -1, -1, 2, 2, 2, -1, 3, 3, 3, 3, -1, -1, 4, 0, 5, -1, -1, -1, -1, 4, -1, 0, -1, 5, -1, -1, 4, -1, -1, 0, -1, -1, 5, -1, -1, -1, -1, 0, -1, -1, -1, 5}, {-1, 6, -1, -1, 1, -1, -1, 7, -1, -1, 6, -1, 1, -1, 7, -1, -1, -1, -1, 6, 1, 7, -1, -1, 2, 2, 2, 2, -1, 3, 3, 3, -1, -1, -1, 4, 0, 5, -1, -1, -1, -1, 4, -1, 0, -1, 5, -1, -1, 4, -1, -1, 0, -1, -1, 5, 4, -1, -1, -1, 0, -1, -1, -1}, {-1, -1, 6, -1, -1, 1, -1, -1, -1, -1, -1, 6, -1, 1, -1, 7, -1, -1, -1, -1, 6, 1, 7, -1, 2, 2, 2, 2, 2, -1, 3, 3, -1, -1, -1, -1, 4, 0, 5, -1, -1, -1, -1, 4, -1, 0, -1, 5, -1, -1, 4, -1, -1, 0, -1, -1, -1, 4, -1, -1, -1, 0, -1, -1}, {-1, -1, -1, 6, -1, -1, 1, -1, -1, -1, -1, -1, 6, -1, 1, -1, -1, -1, -1, -1, -1, 6, 1, 7, 2, 2, 2, 2, 2, 2, -1, 3, -1, -1, -1, -1, -1, 4, 0, 5, -1, -1, -1, -1, 4, -1, 0, -1, -1, -1, -1, 4, -1, -1, 0, -1, -1, -1, 4, -1, -1, -1, 0, -1}, {-1, -1, -1, -1, 6, -1, -1, 1, -1, -1, -1, -1, -1, 6, -1, 1, -1, -1, -1, -1, -1, -1, 6, 1, 2, 2, 2, 2, 2, 2, 2, -1, -1, -1, -1, -1, -1, -1, 4, 0, -1, -1, -1, -1, -1, 4, -1, 0, -1, -1, -1, -1, 4, -1, -1, 0, -1, -1, -1, 4, -1, -1, -1, 0}, {1, -1, -1, -1, 7, -1, -1, -1, 1, -1, -1, 7, -1, -1, -1, -1, 1, -1, 7, -1, -1, -1, -1, -1, 1, 7, -1, -1, -1, -1, -1, -1, -1, 3, 3, 3, 3, 3, 3, 3, 0, 5, -1, -1, -1, -1, -1, -1, 0, -1, 5, -1, -1, -1, -1, -1, 0, -1, -1, 5, -1, -1, -1, -1}, {-1, 1, -1, -1, -1, 7, -1, -1, -1, 1, -1, -1, 7, -1, -1, -1, -1, 1, -1, 7, -1, -1, -1, -1, 6, 1, 7, -1, -1, -1, -1, -1, 2, -1, 3, 3, 3, 3, 3, 3, 4, 0, 5, -1, -1, -1, -1, -1, -1, 0, -1, 5, -1, -1, -1, -1, -1, 0, -1, -1, 5, -1, -1, -1}, {-1, -1, 1, -1, -1, -1, 7, -1, -1, -1, 1, -1, -1, 7, -1, -1, 6, -1, 1, -1, 7, -1, -1, -1, -1, 6, 1, 7, -1, -1, -1, -1, 2, 2, -1, 3, 3, 3, 3, 3, -1, 4, 0, 5, -1, -1, -1, -1, 4, -1, 0, -1, 5, -1, -1, -1, -1, -1, 0, -1, -1, 5, -1, -1}, {-1, -1, -1, 1, -1, -1, -1, 7, 6, -1, -1, 1, -1, -1, 7, -1, -1, 6, -1, 1, -1, 7, -1, -1, -1, -1, 6, 1, 7, -1, -1, -1, 2, 2, 2, -1, 3, 3, 3, 3, -1, -1, 4, 0, 5, -1, -1, -1, -1, 4, -1, 0, -1, 5, -1, -1, 4, -1, -1, 0, -1, -1, 5, -1}, {6, -1, -1, -1, 1, -1, -1, -1, -1, 6, -1, -1, 1, -1, -1, 7, -1, -1, 6, -1, 1, -1, 7, -1, -1, -1, -1, 6, 1, 7, -1, -1, 2, 2, 2, 2, -1, 3, 3, 3, -1, -1, -1, 4, 0, 5, -1, -1, -1, -1, 4, -1, 0, -1, 5, -1, -1, 4, -1, -1, 0, -1, -1, 5}, {-1, 6, -1, -1, -1, 1, -1, -1, -1, -1, 6, -1, -1, 1, -1, -1, -1, -1, -1, 6, -1, 1, -1, 7, -1, -1, -1, -1, 6, 1, 7, -1, 2, 2, 2, 2, 2, -1, 3, 3, -1, -1, -1, -1, 4, 0, 5, -1, -1, -1, -1, 4, -1, 0, -1, 5, -1, -1, 4, -1, -1, 0, -1, -1}, {-1, -1, 6, -1, -1, -1, 1, -1, -1, -1, -1, 6, -1, -1, 1, -1, -1, -1, -1, -1, 6, -1, 1, -1, -1, -1, -1, -1, -1, 6, 1, 7, 2, 2, 2, 2, 2, 2, -1, 3, -1, -1, -1, -1, -1, 4, 0, 5, -1, -1, -1, -1, 4, -1, 0, -1, -1, -1, -1, 4, -1, -1, 0, -1}, {-1, -1, -1, 6, -1, -1, -1, 1, -1, -1, -1, -1, 6, -1, -1, 1, -1, -1, -1, -1, -1, 6, -1, 1, -1, -1, -1, -1, -1, -1, 6, 1, 2, 2, 2, 2, 2, 2, 2, -1, -1, -1, -1, -1, -1, -1, 4, 0, -1, -1, -1, -1, -1, 4, -1, 0, -1, -1, -1, -1, 4, -1, -1, 0}, {1, -1, -1, -1, -1, 7, -1, -1, 1, -1, -1, -1, 7, -1, -1, -1, 1, -1, -1, 7, -1, -1, -1, -1, 1, -1, 7, -1, -1, -1, -1, -1, 1, 7, -1, -1, -1, -1, -1, -1, -1, 3, 3, 3, 3, 3, 3, 3, 0, 5, -1, -1, -1, -1, -1, -1, 0, -1, 5, -1, -1, -1, -1, -1}, {-1, 1, -1, -1, -1, -1, 7, -1, -1, 1, -1, -1, -1, 7, -1, -1, -1, 1, -1, -1, 7, -1, -1, -1, -1, 1, -1, 7, -1, -1, -1, -1, 6, 1, 7, -1, -1, -1, -1, -1, 2, -1, 3, 3, 3, 3, 3, 3, 4, 0, 5, -1, -1, -1, -1, -1, -1, 0, -1, 5, -1, -1, -1, -1}, {-1, -1, 1, -1, -1, -1, -1, 7, -1, -1, 1, -1, -1, -1, 7, -1, -1, -1, 1, -1, -1, 7, -1, -1, 6, -1, 1, -1, 7, -1, -1, -1, -1, 6, 1, 7, -1, -1, -1, -1, 2, 2, -1, 3, 3, 3, 3, 3, -1, 4, 0, 5, -1, -1, -1, -1, 4, -1, 0, -1, 5, -1, -1, -1}, {-1, -1, -1, 1, -1, -1, -1, -1, -1, -1, -1, 1, -1, -1, -1, 7, 6, -1, -1, 1, -1, -1, 7, -1, -1, 6, -1, 1, -1, 7, -1, -1, -1, -1, 6, 1, 7, -1, -1, -1, 2, 2, 2, -1, 3, 3, 3, 3, -1, -1, 4, 0, 5, -1, -1, -1, -1, 4, -1, 0, -1, 5, -1, -1}, {-1, -1, -1, -1, 1, -1, -1, -1, 6, -1, -1, -1, 1, -1, -1, -1, -1, 6, -1, -1, 1, -1, -1, 7, -1, -1, 6, -1, 1, -1, 7, -1, -1, -1, -1, 6, 1, 7, -1, -1, 2, 2, 2, 2, -1, 3, 3, 3, -1, -1, -1, 4, 0, 5, -1, -1, -1, -1, 4, -1, 0, -1, 5, -1}, {6, -1, -1, -1, -1, 1, -1, -1, -1, 6, -1, -1, -1, 1, -1, -1, -1, -1, 6, -1, -1, 1, -1, -1, -1, -1, -1, 6, -1, 1, -1, 7, -1, -1, -1, -1, 6, 1, 7, -1, 2, 2, 2, 2, 2, -1, 3, 3, -1, -1, -1, -1, 4, 0, 5, -1, -1, -1, -1, 4, -1, 0, -1, 5}, {-1, 6, -1, -1, -1, -1, 1, -1, -1, -1, 6, -1, -1, -1, 1, -1, -1, -1, -1, 6, -1, -1, 1, -1, -1, -1, -1, -1, 6, -1, 1, -1, -1, -1, -1, -1, -1, 6, 1, 7, 2, 2, 2, 2, 2, 2, -1, 3, -1, -1, -1, -1, -1, 4, 0, 5, -1, -1, -1, -1, 4, -1, 0, -1}, {-1, -1, 6, -1, -1, -1, -1, 1, -1, -1, -1, 6, -1, -1, -1, 1, -1, -1, -1, -1, 6, -1, -1, 1, -1, -1, -1, -1, -1, 6, -1, 1, -1, -1, -1, -1, -1, -1, 6, 1, 2, 2, 2, 2, 2, 2, 2, -1, -1, -1, -1, -1, -1, -1, 4, 0, -1, -1, -1, -1, -1, 4, -1, 0}, {1, -1, -1, -1, -1, -1, 7, -1, 1, -1, -1, -1, -1, 7, -1, -1, 1, -1, -1, -1, 7, -1, -1, -1, 1, -1, -1, 7, -1, -1, -1, -1, 1, -1, 7, -1, -1, -1, -1, -1, 1, 7, -1, -1, -1, -1, -1, -1, -1, 3, 3, 3, 3, 3, 3, 3, 0, 5, -1, -1, -1, -1, -1, -1}, {-1, 1, -1, -1, -1, -1, -1, 7, -1, 1, -1, -1, -1, -1, 7, -1, -1, 1, -1, -1, -1, 7, -1, -1, -1, 1, -1, -1, 7, -1, -1, -1, -1, 1, -1, 7, -1, -1, -1, -1, 6, 1, 7, -1, -1, -1, -1, -1, 2, -1, 3, 3, 3, 3, 3, 3, 4, 0, 5, -1, -1, -1, -1, -1}, {-1, -1, 1, -1, -1, -1, -1, -1, -1, -1, 1, -1, -1, -1, -1, 7, -1, -1, 1, -1, -1, -1, 7, -1, -1, -1, 1, -1, -1, 7, -1, -1, 6, -1, 1, -1, 7, -1, -1, -1, -1, 6, 1, 7, -1, -1, -1, -1, 2, 2, -1, 3, 3, 3, 3, 3, -1, 4, 0, 5, -1, -1, -1, -1}, {-1, -1, -1, 1, -1, -1, -1, -1, -1, -1, -1, 1, -1, -1, -1, -1, -1, -1, -1, 1, -1, -1, -1, 7, 6, -1, -1, 1, -1, -1, 7, -1, -1, 6, -1, 1, -1, 7, -1, -1, -1, -1, 6, 1, 7, -1, -1, -1, 2, 2, 2, -1, 3, 3, 3, 3, -1, -1, 4, 0, 5, -1, -1, -1}, {-1, -1, -1, -1, 1, -1, -1, -1, -1, -1, -1, -1, 1, -1, -1, -1, 6, -1, -1, -1, 1, -1, -1, -1, -1, 6, -1, -1, 1, -1, -1, 7, -1, -1, 6, -1, 1, -1, 7, -1, -1, -1, -1, 6, 1, 7, -1, -1, 2, 2, 2, 2, -1, 3, 3, 3, -1, -1, -1, 4, 0, 5, -1, -1}, {-1, -1, -1, -1, -1, 1, -1, -1, 6, -1, -1, -1, -1, 1, -1, -1, -1, 6, -1, -1, -1, 1, -1, -1, -1, -1, 6, -1, -1, 1, -1, -1, -1, -1, -1, 6, -1, 1, -1, 7, -1, -1, -1, -1, 6, 1, 7, -1, 2, 2, 2, 2, 2, -1, 3, 3, -1, -1, -1, -1, 4, 0, 5, -1}, {6, -1, -1, -1, -1, -1, 1, -1, -1, 6, -1, -1, -1, -1, 1, -1, -1, -1, 6, -1, -1, -1, 1, -1, -1, -1, -1, 6, -1, -1, 1, -1, -1, -1, -1, -1, 6, -1, 1, -1, -1, -1, -1, -1, -1, 6, 1, 7, 2, 2, 2, 2, 2, 2, -1, 3, -1, -1, -1, -1, -1, 4, 0, 5}, {-1, 6, -1, -1, -1, -1, -1, 1, -1, -1, 6, -1, -1, -1, -1, 1, -1, -1, -1, 6, -1, -1, -1, 1, -1, -1, -1, -1, 6, -1, -1, 1, -1, -1, -1, -1, -1, 6, -1, 1, -1, -1, -1, -1, -1, -1, 6, 1, 2, 2, 2, 2, 2, 2, 2, -1, -1, -1, -1, -1, -1, -1, 4, 0}, {1, -1, -1, -1, -1, -1, -1, 7, 1, -1, -1, -1, -1, -1, 7, -1, 1, -1, -1, -1, -1, 7, -1, -1, 1, -1, -1, -1, 7, -1, -1, -1, 1, -1, -1, 7, -1, -1, -1, -1, 1, -1, 7, -1, -1, -1, -1, -1, 1, 7, -1, -1, -1, -1, -1, -1, -1, 3, 3, 3, 3, 3, 3, 3}, {-1, 1, -1, -1, -1, -1, -1, -1, -1, 1, -1, -1, -1, -1, -1, 7, -1, 1, -1, -1, -1, -1, 7, -1, -1, 1, -1, -1, -1, 7, -1, -1, -1, 1, -1, -1, 7, -1, -1, -1, -1, 1, -1, 7, -1, -1, -1, -1, 6, 1, 7, -1, -1, -1, -1, -1, 2, -1, 3, 3, 3, 3, 3, 3}, {-1, -1, 1, -1, -1, -1, -1, -1, -1, -1, 1, -1, -1, -1, -1, -1, -1, -1, 1, -1, -1, -1, -1, 7, -1, -1, 1, -1, -1, -1, 7, -1, -1, -1, 1, -1, -1, 7, -1, -1, 6, -1, 1, -1, 7, -1, -1, -1, -1, 6, 1, 7, -1, -1, -1, -1, 2, 2, -1, 3, 3, 3, 3, 3}, {-1, -1, -1, 1, -1, -1, -1, -1, -1, -1, -1, 1, -1, -1, -1, -1, -1, -1, -1, 1, -1, -1, -1, -1, -1, -1, -1, 1, -1, -1, -1, 7, 6, -1, -1, 1, -1, -1, 7, -1, -1, 6, -1, 1, -1, 7, -1, -1, -1, -1, 6, 1, 7, -1, -1, -1, 2, 2, 2, -1, 3, 3, 3, 3}, {-1, -1, -1, -1, 1, -1, -1, -1, -1, -1, -1, -1, 1, -1, -1, -1, -1, -1, -1, -1, 1, -1, -1, -1, 6, -1, -1, -1, 1, -1, -1, -1, -1, 6, -1, -1, 1, -1, -1, 7, -1, -1, 6, -1, 1, -1, 7, -1, -1, -1, -1, 6, 1, 7, -1, -1, 2, 2, 2, 2, -1, 3, 3, 3}, {-1, -1, -1, -1, -1, 1, -1, -1, -1, -1, -1, -1, -1, 1, -1, -1, 6, -1, -1, -1, -1, 1, -1, -1, -1, 6, -1, -1, -1, 1, -1, -1, -1, -1, 6, -1, -1, 1, -1, -1, -1, -1, -1, 6, -1, 1, -1, 7, -1, -1, -1, -1, 6, 1, 7, -1, 2, 2, 2, 2, 2, -1, 3, 3}, {-1, -1, -1, -1, -1, -1, 1, -1, 6, -1, -1, -1, -1, -1, 1, -1, -1, 6, -1, -1, -1, -1, 1, -1, -1, -1, 6, -1, -1, -1, 1, -1, -1, -1, -1, 6, -1, -1, 1, -1, -1, -1, -1, -1, 6, -1, 1, -1, -1, -1, -1, -1, -1, 6, 1, 7, 2, 2, 2, 2, 2, 2, -1, 3}, {6, -1, -1, -1, -1, -1, -1, 1, -1, 6, -1, -1, -1, -1, -1, 1, -1, -1, 6, -1, -1, -1, -1, 1, -1, -1, -1, 6, -1, -1, -1, 1, -1, -1, -1, -1, 6, -1, -1, 1, -1, -1, -1, -1, -1, 6, -1, 1, -1, -1, -1, -1, -1, -1, 6, 1, 2, 2, 2, 2, 2, 2, 2, -1}};
        #endregion directionIdx

    }
}
