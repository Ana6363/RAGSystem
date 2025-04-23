import os
from langchain.chains.question_answering import load_qa_chain
from langchain_openai import OpenAI
from dotenv import load_dotenv
from langchain_community.vectorstores import Qdrant
from langchain_openai import OpenAIEmbeddings
from langchain_qdrant import Qdrant
from qdrant_client import QdrantClient

load_dotenv()

OPENAI_API_KEY = os.getenv("OPENAI_API_KEY")
QDRANT_URL = os.getenv("QDRANT_URL")
QDRANT_API_KEY = os.getenv("QDRANT_API_KEY")
QDRANT_COLLECTION = os.getenv("QDRANT_COLLECTION", "nbanks_documents")

embeddings = OpenAIEmbeddings(api_key=OPENAI_API_KEY)

# Official client from the Qdrant Python SDK
# This is used to connect to Qdrant Cloud (url) and authenticate (api_key)
qdrant_client = QdrantClient(
    url=QDRANT_URL,
    api_key=QDRANT_API_KEY,
)

# LanchChain's Qdrant wrapper
# Plugs Qdrant into its vector retrieval system
# It will know which collection to search (collection_name)
# Which embeddings to use (embeddings)
# Where to send the search request (client)
vectorstore = Qdrant(
    client=qdrant_client,
    collection_name=QDRANT_COLLECTION,
    embeddings=embeddings,
)

# Creates a LangChain-compatible wrapper around OpenAI’s API. It tells LangChain:
# This object (llm) knows how to:
# Format prompts
# Make HTTP requests to OpenAI’s API
# Handle the response
llm = OpenAI(temperature=0, openai_api_key=OPENAI_API_KEY)


def query_pdf(file_id: str, question: str) -> str:
    
    # Semantic Search in Qdrant
    # Embeddings are used to find the most relevant chunks
    # The filter is used to limit the search to a specific file_id
    # The k parameter is used to limit the number of chunks returned
    relevant_docs = vectorstore.similarity_search(
        query=question,
        k=4,
        filter={"file_id": file_id}
    )

    if not relevant_docs:
        raise ValueError(f"No chunks found for file_id: {file_id}")

    # Load the QA chain
    chain = load_qa_chain(llm, chain_type="stuff") # Loads a prebuilt chain template for question answering.
    result = chain.run(input_documents=relevant_docs, question=question) # Runs the chain with the loaded documents and the user's question.
    return result

"""

 def query_pdfs(file_ids: list, question: str) -> str:
    # Initialize an empty list to collect relevant documents
    all_relevant_docs = []

    # Iterate over all file_ids and search for relevant chunks
    for file_id in file_ids:
        relevant_docs = vectorstore.similarity_search(
            query=question,
            k=4,
            filter={"file_id": file_id}
        )

        if relevant_docs:
            all_relevant_docs.extend(relevant_docs)
        else:
            print(f"No chunks found for file_id: {file_id}")

    if not all_relevant_docs:
        raise ValueError("No relevant chunks found in any of the provided file_ids.")

    # Load the QA chain
    chain = load_qa_chain(llm, chain_type="stuff")  # Loads a prebuilt chain template for question answering
    result = chain.run(input_documents=all_relevant_docs, question=question)  # Runs the chain with the combined documents and the user's question

    return result 

"""