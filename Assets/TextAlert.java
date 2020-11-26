/**
 *  Copyright (c) 2018 Devon O. Wolfgang
 *
 *  Permission is hereby granted, free of charge, to any person obtaining a copy
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

package com.onebyonedesign.unityplugins;

import android.app.Activity;
import android.util.Log;
import android.app.AlertDialog;
import android.content.DialogInterface;
import android.view.WindowManager;

public class TextAlert
{
    private static final String TAG = "TextAlert";

    /** Create a new TextAlert */
    private TextAlert(){}

    /** Get Singleton instance of TextAlert */
    public static TextAlert getInstance()
    {
        return SingletonHelper.INSTANCE;
    }

    /** Display passed string in Alert Dialog */
    public void show(Activity a, String msg)
    {
        Log.i(TAG, "Showing alert ("+msg+")");

        AlertDialog d = new AlertDialog
            .Builder(a)
            .setMessage(msg)
            .setPositiveButton("OK", new DialogInterface.OnClickListener(){
                public void onClick(DialogInterface dialog, int id) {
                    dialog.dismiss();
                }
            })
            .create();
        d.setCancelable(false);
        d.setCanceledOnTouchOutside(false);
        d.getWindow()
            .setFlags(WindowManager.LayoutParams.FLAG_NOT_FOCUSABLE, WindowManager.LayoutParams.FLAG_NOT_FOCUSABLE);
        d.show();
    }

    // Inner class singleton helper
    private static class SingletonHelper
    {
        private static final TextAlert INSTANCE = new TextAlert();
    }
}