#!/usr/bin/env python
# coding: utf-8

# In[34]:


import tensorflow as tf
import tensorflow_hub as hub
import matplotlib.pyplot as plt
import numpy as np
import os
import pandas as pd
import re
import seaborn as sns
import math
import sys
module_url = "https://tfhub.dev/google/universal-sentence-encoder/2" #@param ["https://tfhub.dev/google/universal-sentence-encoder/2", "https://tfhub.dev/google/universal-sentence-encoder-large/3"]

# Import the Universal Sentence Encoder's TF Hub module
embed = hub.Module(module_url)


# Compute a representation for each message, showing various lengths supported.
List11=sys.argv[1]
List22=sys.argv[2]
List1=["hello","Cat","Dog","Monkey","Spoon","Fork"]
List2=["Wall","Tiger","Hammer","Mouse","Shirt"]
print(List11)
print(List22)
print(List1)
print(List2)
Embeddings=[]
messages=List1

# Reduce logging output.
tf.logging.set_verbosity(tf.logging.ERROR)

for y in range(2):
    if y==1:
        messages=List2
    
    with tf.Session() as session:
      session.run([tf.global_variables_initializer(), tf.tables_initializer()])
      message_embeddings = session.run(embed(messages))
    
      for i, message_embedding in enumerate(np.array(message_embeddings).tolist()):
        print("Message: {}".format(messages[i]))
        print("Embedding size: {}".format(len(message_embedding)))
        message_embedding_snippet = ", ".join(
            (str(x) for x in message_embedding[:3]))
        print("Embedding: [{}, ...]\n".format(message_embedding_snippet))
        temp=message_embedding_snippet
        Embeddings.append(temp)
print(Embeddings)
        


# In[37]:


list1Embedding=[]
list2Embedding=[]
i=1;
for x in Embeddings:
    temp=x.split(",")
    for f in range(len(temp)):
        temp[f]=float(temp[f])
    
    if(i<=len(List1)):
        list1Embedding.append(temp)
    else:
        list2Embedding.append(temp)
    i+=1
encodingsum=0
for x in range(len(list1Embedding)):
    for y in range(len(list2Embedding)):
        print(list1Embedding[x])
        print(list2Embedding[y])
        print("\n")
        for f in range(len(list1Embedding[x])):
            encodingsum+=((list1Embedding[x][f]-list2Embedding[y][f])**2)
        #distance = math.sqrt(sum([(a - b) ** 2 for a, b in zip(x, y)]))
        distance=math.sqrt(encodingsum)
        print("Distance between "+List1[x]+" and "+List2[y]+" is "+str(distance))

def plot_similarity(labels, features, rotation):
  corr = np.inner(features, features)
  sns.set(font_scale=1.2)
  g = sns.heatmap(
      corr,
      xticklabels=labels,
      yticklabels=labels,
      vmin=0,
      vmax=1,
      cmap="YlOrRd")
  g.set_xticklabels(labels, rotation=rotation)
  g.set_title("Semantic Textual Similarity")
  g.figure.savefig("output.png")


def run_and_plot(session_, input_tensor_, messages_, encoding_tensor):
  message_embeddings_ = session_.run(
      encoding_tensor, feed_dict={input_tensor_: messages_})
  plot_similarity(messages_, message_embeddings_, 90)
messages=List1+List2
similarity_input_placeholder = tf.placeholder(tf.string, shape=(None))
similarity_message_encodings = embed(similarity_input_placeholder)
with tf.Session() as session:
  session.run(tf.global_variables_initializer())
  session.run(tf.tables_initializer())
  run_and_plot(session, similarity_input_placeholder, messages,
               similarity_message_encodings)
  

