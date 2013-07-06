package com.zgs.bean;

import javax.servlet.ServletException;
import javax.xml.parsers.DocumentBuilderFactory;

import java.io.IOException;
import java.lang.IllegalArgumentException;
import javax.xml.parsers.DocumentBuilder;
import org.w3c.dom.Document;
import org.w3c.dom.Node;
import org.w3c.dom.NodeList;
import org.apache.log4j.Logger;

public class GameResult {
	private String username;
	private String version;
	private int score;
	private int duration;
	private int level;
	private String detail;
	private String gamename;
	
	public static final String usernameTag = "username";
	public static final String versionTag = "version";
	public static final String scoreTag = "score";
	public static final String durationTag = "duration";
	public static final String levelTag = "level";
	public static final String detailTag = "detail";
	public static final String gamenameTag = "name";

	static Logger logger = Logger.getLogger(GameResult.class.getName());
	
	public GameResult(){
		
	}
	
	/**
	 * Compare this gameResult with another one. <br>
	 * 
	 * @param other
	 *            another game result to be compared with
	 * @return 
	 *            1 means this one is larger than other
	 *            -1 means this one is smaller than other
	 *            0 means the two game results are equal
	 */
	public int compareWith(GameResult other){
		if(score > other.getScore())
			return 1;
		else if(score < other.getScore())
			return -1;
		else if(level > other.getLevel())
			return 1;
		else if(level < other.getLevel())
			return -1;
		else if(duration > other.getDuration())
			return 1;
		else if(duration < other.getDuration())
			return -1;
		else
			return 0;
	}
	
	/*public GameResult(String s) throws Exception{
		try { 
			DocumentBuilderFactory dbf = DocumentBuilderFactory.newInstance(); 
			DocumentBuilder db = dbf.newDocumentBuilder(); 
			Document document = db.parse(s); 
			Node resultNode = document.getFirstChild();
			if(resultNode.getNodeName() != gameResultTag){
				throw new IllegalArgumentException(String.format("The first tag of game result should be %s", gameResultTag));
			}
			
			NodeList nodes = resultNode.getChildNodes();
			for(int i = 0; i < nodes.getLength(); i++){
				Node node = nodes.item(i);
				String nodeName = node.getNodeName();
				switch(nodeName)
				{
				case usernameTag:
					this.setUsername(node.getNodeValue());
					break;
				case versionTag:
					this.setVersion(node.getNodeValue());
					break;
				case scoreTag:
					this.setScore(Integer.parseInt(node.getNodeValue()));
					break;
				case durationTag:
					this.setDuration(Integer.parseInt(node.getNodeValue()));
					break;
				case levelTag:
					this.setLevel(Integer.parseInt(node.getNodeValue()));
					break;
				case detailTag:
					this.setDetail(node.getNodeValue());
					break;
				case gamenameTag:
					this.setGamename(node.getNodeValue());
					break;
				}
			}
		}
		catch(Exception e){
			logger.warn(String.format("Got exception when parsing game result xml %s.\n  Detail exception = %s", s, e.toString()));
			throw e;
		}
	}*/
	
	public String getUsername() {
		return username;
	}
	public String getGamename() {
		return gamename;
	}

	public void setGamename(String gamename) {
		this.gamename = gamename;
	}

	public void setUsername(String username) {
		this.username = username;
	}
	public int getScore() {
		return score;
	}
	public void setScore(int score) {
		this.score = score;
	}
	public int getDuration() {
		return duration;
	}
	public void setDuration(int duration) {
		this.duration = duration;
	}
	public int getLevel() {
		return level;
	}
	public void setLevel(int level) {
		this.level = level;
	}
	public String getDetail() {
		return detail;
	}
	public void setDetail(String detail) {
		this.detail = detail;
	}
	public String getVersion() {
		return version;
	}
	public void setVersion(String version) {
		this.version = version;
	}
}
