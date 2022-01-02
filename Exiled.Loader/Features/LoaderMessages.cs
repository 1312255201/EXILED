﻿// -----------------------------------------------------------------------
// <copyright file="LoaderMessages.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Loader.Features
{
    using System;

    /// <summary>
    /// A class that contains the different EXILED loader messages.
    /// </summary>
    public static class LoaderMessages
    {
        /// <summary>
        /// Gets the default loader message.
        /// </summary>
        public static string Default => @"
   ▄████████ ▀████    ▐████▀  ▄█   ▄█          ▄████████ ████████▄
  ███    ███   ███▌   ████▀  ███  ███         ███    ███ ███   ▀███
  ███    █▀     ███  ▐███    ███▌ ███         ███    █▀  ███    ███
 ▄███▄▄▄        ▀███▄███▀    ███▌ ███        ▄███▄▄▄     ███    ███
▀▀███▀▀▀        ████▀██▄     ███▌ ███       ▀▀███▀▀▀     ███    ███
  ███    █▄    ▐███  ▀███    ███  ███         ███    █▄  ███    ███
  ███    ███  ▄███     ███▄  ███  ███▌    ▄   ███    ███ ███   ▄███
  ██████████ ████       ███▄ █▀   █████▄▄██   ██████████ ████████▀
                                  ▀                                 ";

        /// <summary>
        /// Gets the christmas loader message.
        /// </summary>
        public static string Christmas => @"
       __
    .-'  |
   /   <\|        ▄████████ ▀████    ▐████▀  ▄█   ▄█          ▄████████ ████████▄
  /     \'       ███    ███   ███▌   ████▀  ███  ███         ███    ███ ███   ▀███
  |_.- o-o       ███    █▀     ███  ▐███    ███▌ ███         ███    █▀  ███    ███
  / C  -._)\    ▄███▄▄▄        ▀███▄███▀    ███▌ ███        ▄███▄▄▄     ███    ███
 /',        |  ▀▀███▀▀▀        ████▀██▄     ███▌ ███       ▀▀███▀▀▀     ███    ███
|   `-,_,__,'    ███    █▄    ▐███  ▀███    ███  ███         ███    █▄  ███    ███
(,,)====[_]=|    ███    ███  ▄███     ███▄  ███  ███▌    ▄   ███    ███ ███   ▄███
  '.   ____/     ██████████ ████       ███▄ █▀   █████▄▄██   ██████████ ████████▀
   | -|-|_
   |____)_)";

        /// <summary>
        /// Gets the loader message according to the actual month.
        /// </summary>
        /// <returns>The correspondent loader message.</returns>
        public static string GetMessage()
        {
            switch (DateTime.Today.Month)
            {
                case 12:
                    return Christmas;
                default:
                    return Default;
            }
        }
    }
}
