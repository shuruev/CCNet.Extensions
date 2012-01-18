﻿<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl">

	<xsl:include href="Styles.xslt"/>

	<xsl:param name="cutoffDays"/>

	<xsl:template match="change">
		<xsl:if test="position() &gt; 1">
			<p class="spacer" style="font-size:0.5pt">&#160;</p>
		</xsl:if>
		<p class="change">
			<nobr>
				<span class="filename">
					<xsl:value-of select="path"/>
				</span>
				<xsl:text>&#160;</xsl:text>
				<span class="fileage">
					<xsl:text>(</xsl:text>
					<xsl:value-of select="daysSinceCheckout"/>
					<xsl:text> days ago)</xsl:text>
				</span>
			</nobr>
		</p>
	</xsl:template>

	<xsl:template match="userInfo" mode="userActivityClass">
		<xsl:choose>
			<xsl:when test="not(isRegistered = 'true')">passive</xsl:when>
			<xsl:when test="contains(description, 'Fired')">passive</xsl:when>
			<xsl:when test="not(daysSinceLastLogon &lt; 30)">passive</xsl:when>
			<xsl:otherwise>active</xsl:otherwise>
		</xsl:choose>
	</xsl:template>

	<xsl:template match="userInfo" mode="userCard">
		<div>
			<xsl:attribute name="class">
				<xsl:text>user</xsl:text>
				<xsl:text> </xsl:text>
				<xsl:apply-templates select="current()" mode="userActivityClass"/>
			</xsl:attribute>
			<p class="name">
				<xsl:value-of select="displayName"/>
			</p>
			<p class="spacer" style="font-size:2.7pt;">&#160;</p>
			<p class="description">
				<xsl:choose>
					<xsl:when test="isRegistered = 'true'">
						<xsl:value-of select="description"/>
					</xsl:when>
					<xsl:otherwise>
						<font color="red">Not registered</font>
					</xsl:otherwise>
				</xsl:choose>
			</p>
		</div>
	</xsl:template>

	<xsl:template match="/root">
		<html>
			<head>
				<xsl:call-template name="defaultStyle"/>
			</head>
			<body>
				<xsl:apply-templates select="current()" mode="body"/>
			</body>
		</html>
	</xsl:template>

	<xsl:template match="group" mode="groupCard">
		<xsl:if test="position() &gt; 1">
			<p class="spacer" style="font-size:15pt">&#160;</p>
			<hr/>
			<p class="spacer" style="font-size:15pt">&#160;</p>
		</xsl:if>
		<div class="groupcard">
			<xsl:apply-templates select="userInfo" mode="userCard"/>
			<p class="spacer" style="font-size:10pt;">&#160;</p>
			<p>
				<xsl:text>There are </xsl:text>
				<strong>
					<xsl:value-of select="count(change)"/>
				</strong>
				<xsl:text> pending changes made more than </xsl:text>
				<strong>
					<xsl:value-of select="$cutoffDays"/>
				</strong>
				<xsl:text> days ago:</xsl:text>
			</p>
			<p class="spacer" style="font-size:10pt;">&#160;</p>
			<table class="changelist">
				<tr>
					<td>
						<xsl:apply-templates select="change"/>
					</td>
				</tr>
			</table>
		</div>
	</xsl:template>

</xsl:stylesheet>
