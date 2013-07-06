package com.zgs.threads;

import com.zgs.bean.GlobalSettings;

public class CacheRefreshThread {

	public void run(){
		while(GlobalSettings.isRunning()){
			//If other thread is updating the cache, wait
			//Check if there is new record in db (the biggest value in db < the last one in cache)
			//If there is new record, load 10000 record, and rebuild the cache b+tree
			//Sleep 5 minutes
		}
			
	}
	
}
