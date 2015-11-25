<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl">
	<xsl:output method="xml"/>

	<xsl:include href="ReportCommon.xslt"/>

	<xsl:template match="group" mode="userGreeting">
		<p>
			<xsl:text>Dear </xsl:text>
			<xsl:value-of select="userInfo/firstName"/>
			<xsl:text>,</xsl:text>
		</p>
		<p class="spacer" style="font-size:10pt">&#160;</p>
		<p>Please be informed that some files are still marked as "checked-out" in Team Foundation Server.</p>
		<p class="spacer" style="font-size:1pt">&#160;</p>
		<xsl:text>You might want to "check-in" them, if you do not need them anymore.</xsl:text>
		<p class="spacer" style="font-size:22pt">&#160;</p>
		<xsl:apply-templates select="current()" mode="groupCard"/>
	</xsl:template>

	<xsl:template match="/root" mode="body">
		<xsl:apply-templates select="group" mode="userGreeting"/>
	</xsl:template>

</xsl:stylesheet>
