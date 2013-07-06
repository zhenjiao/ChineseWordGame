package com.zgs.servlet;

import java.io.IOException;
import java.io.PrintWriter;
import java.util.Enumeration;

import javax.servlet.ServletException;
import javax.servlet.http.HttpServlet;
import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import com.zgs.bean.GameResult;
import com.zgs.dao.GameTypeDAO;

public class AddGameTypeAction extends HttpServlet{

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
		String gameName = null;
		Enumeration params = request.getParameterNames();
		while(params.hasMoreElements()){
			String name = (String)params.nextElement();
			String value = request.getParameter(name);
			paramString += name + "=" + value + ", ";
			if(name.equals(GameTypeDAO.gamenameTag)){
				gameName = value;
			}
		}
		GameTypeDAO game = new GameTypeDAO();
		int gameId = game.add(gameName);
		if(gameId < 0){
			out.print(String.format("Duplicate game type when adding new gametype to DB.\n parameters are %s", paramString));
		}
		else{
		out.print(String.format("Success. id = %d", gameId));
		}
		}
		catch(Exception e)
		{
		out.print(String.format("Got unhandled exception %s in AddGameTypeAction.\n Parameters are %s", e.toString(), paramString));
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
	
}
