from email.message import Message
import os
import string
import sys
import discord
import asyncio
import argparse

TOKEN = os.getenv('DISCORD_TOKEN')
CRANE = os.getenv('DISCORD_USER')
RELEASE_CHANNEL_ID = 957073637964386314


parser = argparse.ArgumentParser(description="notify for Vapor Trails builds + releases")
parser.add_argument("-b", "--build")
parser.add_argument("-r", "--release", action="store_true", default=False)
args = parser.parse_args()

client = discord.Client()

@client.event
async def on_ready():
	print("busybox online")
	if (args.release):
		print("sending release notification")
		channel = client.get_channel(RELEASE_CHANNEL_ID)
		await channel.send("build "+args.build+" is live at https://sevencrane.itch.io/vapor-trails")
		os._exit(0)

	elif (args.build):
		print("sending build completion message")
		craniel = await client.fetch_user(CRANE)
		await craniel.send("build "+args.build+" is done! please acknowledge")
		await asyncio.sleep(60 * 5)
		await craniel.send("PLEASE")
		await asyncio.sleep(60 * 5)
		await craniel.send("you took longer than ten minutes to respond! I'm killing myself now!!")
		os._exit(0)
	

@client.event
async def on_message(message):
	if is_rando(message):
		await message.reply("fuck you")
		return
	elif not is_self(message) and args.build:
		await message.reply("thanks! killing myself now")
		os._exit(0)


def is_rando(message: Message) -> bool:
	return (message.author.id == CRANE or not args.build) and not is_self(message) and (was_tagged(message) or not message.guild)


def is_self(message: Message) -> bool:
	return message.author.id == client.user.id


def was_tagged(message: Message) -> bool:
	for member in message.mentions:
		if member.id == client.user.id:
			return True
	return False


client.run(TOKEN)
