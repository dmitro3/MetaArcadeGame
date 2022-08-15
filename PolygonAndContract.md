## Polygon mumbai

### Contract Address : 0xE78261e9352231a39af92B619C02889E78eaaE7F
### Blockchain: Polygon Mumbai - Testnet
### Explore : https://mumbai.polygonscan.com/address/0xE78261e9352231a39af92B619C02889E78eaaE7F


#### Script : https://github.com/MetaArcade/MetaArcadeGame/blob/main/MetaArcadeGameSourceCode/Assets/SCripts/BlockChainManager.cs

### Polygon smart contract for
* in-app purchase with matic
* mint NFT as Achivements and Themes

``` c#
    // address of contract
    const string contract = "0xE78261e9352231a39af92B619C02889E78eaaE7F";
    const string chain = "polygon";
    // set network mainnet, testnet
    const string network = "testnet";
    const string chainId = "80001";
``` 
### Polygon smart contract source code

``` ruby
  // SPDX-License-Identifier: MIT
pragma solidity ^0.8.7;

//Importing ERC 1155 Token contract from OpenZeppelin
import "https://github.com/OpenZeppelin/openzeppelin-contracts/blob/master/contracts/token/ERC1155/ERC1155.sol";
import "https://github.com/OpenZeppelin/openzeppelin-contracts/blob/master/contracts/access/Ownable.sol";
import "https://github.com/OpenZeppelin/openzeppelin-contracts/blob/master/contracts/utils/Strings.sol";


contract MetaFunArcade is ERC1155 , Ownable  {
    
    string constant public name = "Meta Fun Arcade Game";
    string constant diBx1DF = "XR0gf";


    mapping(uint256 => string) _tokenUrls;
    
    uint256[] itemCost = [0.025 ether, 0.050 ether, 0.075 ether, 0.1 ether];
    uint256[] nonburnableNFT = [400,401,402,403,500,501,502,503];

    constructor() ERC1155("")  {}

//purchase coins with matic
    function buyCoins(uint256 _itemId) payable public /*onlyOwner*/{
        //IMPORTANT Implement own security (set ownership to users). Not production ready contract
        require(_itemId <= itemCost.length , "invalid item");
        require(msg.value == itemCost[_itemId], "Not enough matic");
    }

//buy burnable nft
    function buyNonBurnItem(uint256 _tokenId, string memory _tokenUrl) public /*onlyOwner*/{
        //IMPORTANT Implement own security (set ownership to users). Not production ready contract
        require(_tokenId <= nonburnableNFT.length , "invalid item");
        _tokenUrls[nonburnableNFT[_tokenId]] = _tokenUrl;
        _mint(msg.sender, nonburnableNFT[_tokenId], 1, "");
    }

    
function getCurrentTime() public view returns(uint256 _result){
    return _result = block.timestamp;
}
 

    function uri(uint256 id) public view virtual override returns (string memory) {
        return _tokenUrls[id];
    }


    function withdraw(address _recipient) public payable onlyOwner {
    payable(_recipient).transfer(address(this).balance);
}
}
``` 

![Polygon use](/Images/PolygonContract.png)
