/**
 *	Copyright (c) 2018 Devon O. Wolfgang
 *
 *	Permission is hereby granted, free of charge, to any person obtaining a copy
 *	of this software and associated documentation files (the "Software"), to deal
 *	in the Software without restriction, including without limitation the rights
 *	to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 *	copies of the Software, and to permit persons to whom the Software is
 *	furnished to do so, subject to the following conditions:
 *
 *	The above copyright notice and this permission notice shall be included in
 *	all copies or substantial portions of the Software.
 *
 *	THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 *	IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 *	FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 *	AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 *	LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 *	OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 *	THE SOFTWARE.
 */

using UnityEngine;

public static class TextAlert
{
    /// <summary>
    /// Show the passed string in a native alert dialog
    /// </summary>
    public static void Show(string msg)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        using(var playerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            AndroidJavaObject activity = playerClass.GetStatic<AndroidJavaObject>("currentActivity");
            using(var pluginClass = new AndroidJavaClass("com.onebyonedesign.unityplugins.TextAlert"))
            {
                pluginClass.CallStatic<AndroidJavaObject>("getInstance")
                    .Call("show", new object[] { activity, msg });
            }
        }
#endif
        Debug.Log("TextAlert.Show("+msg+")");
    }
}