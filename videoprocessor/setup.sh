# echo "Updating all packages..."
# apt-get update && apt-get upgrade -y
# echo "Updating all packages completed."

echo "Setting up Azure CLI..."
curl -sL https://aka.ms/InstallAzureCLIDeb | bash
echo "Setting up Azure CLI completed."

echo "az login..."
az login --service-principal -u spnappid -p spnapppwd --tenant tenatid
az account set --subscription "subscriptionid"
az group list -o table

















# for future use -copy below somewhere

# update all
#RUN apt-get update && apt-get upgrade -y




# install required packages
# RUN apt-get install -y -qq --no-install-recommends \
#     apt-transport-https \
#     apt-utils \
#     ca-certificates \
#     curl \
#     unzip \
#     wget \
#     git \
#     iputils-ping \
#     jq \
#     lsb-release \
#     software-properties-common \ 
#     sudo \
#     ghostscript \
#     freetype2-doc

# # install git lfs extension
# RUN curl -s https://packagecloud.io/install/repositories/github/git-lfs/script.deb.sh | bash
# RUN apt-get install git-lfs -y -qq --no-install-recommends

# # install PowerShell
# RUN wget -q https://github.com/PowerShell/PowerShell/releases/download/v7.3.6/powershell_7.3.6-1.deb_amd64.deb
# RUN dpkg -i powershell_7.3.6-1.deb_amd64.deb
# RUN apt-get install -f

# # Install Terraform
# RUN wget -q https://releases.hashicorp.com/terraform/1.6.2/terraform_1.6.2_linux_amd64.zip \
#     && unzip terraform_1.6.2_linux_amd64.zip \
#     && mv terraform /usr/local/bin/ \
#     && rm terraform_1.6.2_linux_amd64.zip

# # install AzDoURE CLI
# RUN curl -sL https://aka.ms/InstallAzureCLIDeb | bash