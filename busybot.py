import os
import sys
import discord
import asyncio

TOKEN = os.getenv('DISCORD_TOKEN')
CRANE = os.getenv('DISCORD_USER')

client = discord.Client()

@client.event
async def on_ready():
	print("busybox online")
	craniel = await client.fetch_user(CRANE)
	await craniel.send("build "+sys.argv[1]+" is done! please acknowledge")
	await asyncio.sleep(60 * 5)
	await craniel.send("PLEASE")
	await asyncio.sleep(60 * 5)
	await craniel.send("you took longer than ten minutes to respond! I'm killing myself now!")
	os._exit(0)
	

@client.event
async def on_message(message):
	if message.author == client.user:
		return
	
	await message.reply("thanks! killing myself now")
	os._exit(0)

client.run(TOKEN)
