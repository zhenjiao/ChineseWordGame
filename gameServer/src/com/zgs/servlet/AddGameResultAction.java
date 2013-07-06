package com.zgs.servlet;

import java.io.IOException;
import java.io.PrintWriter;
import java.util.Enumeration;

import javax.servlet.ServletException;
import javax.servlet.http.HttpServlet;
import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import com.zgs.dao.GameResultDAO;
import com.zgs.bean.GameResult;

public class AddGameResultAction extends HttpServlet {

	/**
	 * 
	 */
	private static final long serialVersionUID = 7442481908824372565L;

	@Override
	public void doGet(HttpServletRequest request, HttpServletResponse response)
			throws ServletException, IOException {
		response.setContentType("text/html");
		request.setCharacterEncoding("UTF-8");
		response.setCharacterEncoding("UTF-8");
		PrintWriter out = response.getWriter();
		String paramString = "";
		try
		{
		//ServletOutputStream out = response.getOutputStream();
		GameResult result = new GameResult();
		Enumeration params = request.getParameterNames();
		while(params.hasMoreElements()){
			String name = (String)params.nextElement();
			String value = request.getParameter(name);
			paramString += name + "=" + value + ", ";
			switch(name){
			case GameResult.detailTag:
				result.setDetail(value);
				break;
			case GameResult.durationTag:
				result.setDuration(Integer.parseInt(value));
				break;
			case GameResult.gamenameTag:
				result.setGamename(value);
				break;
			case GameResult.levelTag:
				result.setLevel(Integer.parseInt(value));
				break;
			case GameResult.scoreTag:
				result.setScore(Integer.parseInt(value));
				break;
			case GameResult.usernameTag:
				result.setUsername(value);
				break;
			case GameResult.versionTag:
				result.setVersion(value);
				break;
			}
		}
		//TODO: use cache to improve the performance
		//Get the rank from the b+ tree in memory, sort by score, level and time
		//If > 10000, then drop the result and return 10000+
		//Else, if < 100, check if there is no data inserted into db (by timestamp)
		//If yes, load the top 100 scores from db, to get accurate rank in top 100 scores
		//Insert the new score to memory b+ tree
		//enqueue the new score (top 100 go to high pri queue, others go to low pri queue)
		GameResultDAO game = new GameResultDAO();
		int rank = game.add(result);
		if(rank < 0){
			out.print(String.format("Got rank<0 when adding game record to DB.\n parameters are %s", paramString));
		}
		else{
		out.print(String.format("Success. Rank = %d", rank));
		}
		}
		catch(Exception e)
		{
		out.print(String.format("Got unhandled exception %s in AddGameResultAction.\n Parameters are %s", e.toString(), paramString));
		}
		finally
		{
		out.flush();
		out.close();
		}
	}
	
	/**
	 * The doPost method of the servlet. <br>
	 * 
	 * This method is called when a form has its tag value method equals to
	 * post.
	 * 
	 * @param request
	 *            the request send by the client to the server
	 * @param response
	 *            the response send by the server to the client
	 * @throws ServletException
	 *             if an error occurred
	 * @throws IOException
	 *             if an error occurred
	 */
	@Override
	public void doPost(HttpServletRequest request, HttpServletResponse response)
			throws ServletException, IOException {

		this.doGet(request, response);
	}
	
	/**
	 * Initialization of the servlet. <br>
	 * 
	 * @throws ServletException
	 *             if an error occurs
	 */
	@Override
	public void init() throws ServletException {	
		//TODO: initialize the cache threads
	}

}
