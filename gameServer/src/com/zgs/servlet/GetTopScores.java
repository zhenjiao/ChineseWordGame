package com.zgs.servlet;

import java.io.IOException;
import java.io.PrintWriter;
import java.util.Enumeration;

import javax.servlet.ServletException;
import javax.servlet.http.HttpServlet;
import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import com.zgs.bean.GameResult;
import com.zgs.dao.GetTopScoresDAO;

public class GetTopScores extends HttpServlet {

	/**
	 * 
	 */
	private static final long serialVersionUID = -5481033130524541655L;

	@Override
	public void doGet(HttpServletRequest request, HttpServletResponse response)
			throws ServletException, IOException {
		response.setContentType("text/html");
		request.setCharacterEncoding("UTF-8");
		response.setCharacterEncoding("UTF-8");
		PrintWriter out = response.getWriter();
		try
		{
		//ServletOutputStream out = response.getOutputStream();
			GetTopScoresDAO scoreGetter = new GetTopScoresDAO();
			Enumeration params = request.getParameterNames();
			String paramString = "";
			while(params.hasMoreElements()){
				String name = (String)params.nextElement();
				String value = request.getParameter(name);
				paramString += name + "=" + value + ", ";
			}
			GameResult[] scores = scoreGetter.get(request.getParameter("gameName"));
			out.print("Success\n");
			for(int i = 0; i < scores.length; i++)
			{
				out.print(String.format("%s,%d,%d,%d\n", 
						scores[i].getUsername(), 
						scores[i].getScore(),
						scores[i].getLevel(),
						scores[i].getDuration()));
			}
		}
		catch(Exception e)
		{
		out.print(e.toString());
		}
		finally
		{
		out.flush();
		out.close();
		}
	}
}
