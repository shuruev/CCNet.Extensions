<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl"
>
    <xsl:output method="text"/>

	<xsl:variable name="newLine">
		<xsl:text>&#xd;&#xa;</xsl:text>
	</xsl:variable>
	
	<xsl:template match="userInfo">
		<xsl:value-of select="displayName"/>
		<xsl:value-of select="$newLine"/>
		<xsl:choose>
			<xsl:when test="isRegistered = 'true'">
				<xsl:value-of select="description"/>
				<xsl:text>, email </xsl:text>
				<xsl:value-of select="email"/>
				<xsl:text>.</xsl:text>
				<xsl:if test="isLockedOut = 'true'">
					<xsl:text> </xsl:text>
					<xsl:text>User is locked out.</xsl:text>
				</xsl:if>
				<xsl:if test="lastLogon">
					<xsl:text> </xsl:text>
					<xsl:text>Last logon time: </xsl:text>
					<xsl:value-of select="lastLogon"/>
					<xsl:text>, </xsl:text>
					<xsl:value-of select="daysSinceLastLogon"/>
					<xsl:text> days ago.</xsl:text>
				</xsl:if>
				<xsl:value-of select="$newLine"/>
			</xsl:when>
			<xsl:otherwise>
				<xsl:text>Not registered</xsl:text>
				<xsl:value-of select="$newLine"/>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>

	<xsl:template match="change">
		<xsl:value-of select="path"/>
		<xsl:text> (</xsl:text>
		<xsl:value-of select="checkoutDate"/>
		<xsl:text>, </xsl:text>
		<xsl:value-of select="daysSinceCheckout"/>
		<xsl:text> days ago)</xsl:text>
		<xsl:value-of select="$newLine"/>
	</xsl:template>

	<xsl:template match="group">
		<xsl:text>=======================================</xsl:text>
		<xsl:value-of select="$newLine"/>
		
		<xsl:apply-templates select="userInfo"/>
		<xsl:value-of select="$newLine"/>
		
		<xsl:text>Total </xsl:text>
		<xsl:value-of select="count(change)"/>
		<xsl:text> pending old changes.</xsl:text>
		<xsl:value-of select="$newLine"/>

		<xsl:apply-templates select="change"/>
	</xsl:template>

	<xsl:template match="/root">
		<xsl:apply-templates select="*"/>
	</xsl:template>

</xsl:stylesheet>
